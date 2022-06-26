using Modding;
using Satchel;
using System.Collections.Generic;
using UnityEngine;
using GlobalEnums;
using Random = System.Random;

namespace DeadQuirrel
{
	public class DeadQuirrel : Mod, IGlobalSettings<Settings>
	{
		public static DeadQuirrel Instance;
		private GameObject quirrelGO;
		private Random random;

		public static Settings Settings { get; set; } = new Settings();
		public void OnLoadGlobal(Settings s) => Settings = s;
		public Settings OnSaveGlobal() => Settings;

		public override string GetVersion() => AssemblyUtils.GetAssemblyVersionHash();
		public DeadQuirrel() : base("DeadQuirrel") { }

		public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
		{
			Log("Initializing");

			quirrelGO = new GameObject("Dead Quirrel");
			UnityEngine.Object.DontDestroyOnLoad(quirrelGO);
			quirrelGO.layer = (int)PhysLayers.ENEMIES;

			SpriteRenderer sr = quirrelGO.AddComponent<SpriteRenderer>();
			if(Settings.masked)
			{
				quirrelGO.GetComponent<SpriteRenderer>().sprite = AssemblyUtils.GetSpriteFromResources("mask.png");
			}
			else
			{
				sr.sprite = AssemblyUtils.GetSpriteFromResources("nomask.png");
			}
			sr.size = sr.sprite.rect.size;
			sr.enabled = true;

			CircleCollider2D cc = quirrelGO.AddComponent<CircleCollider2D>();
			cc.radius = 0.6f;
			cc.enabled = true;

			Rigidbody2D rb = quirrelGO.AddComponent<Rigidbody2D>();
			rb.gravityScale = 0f;

			quirrelGO.SetActive(false);

			random = new Random();

			ModHooks.BeforeSceneLoadHook += BeforeSceneLoadHook;

			Log("Initialized");
		}

		private string BeforeSceneLoadHook(string scene)
		{
			if(scene == "Crossroads_50" && PlayerData.instance.GetBool(nameof(PlayerData.quirrelEpilogueCompleted)))
			{
				quirrelGO.SetActive(true);
				quirrelGO.transform.position = new Vector3(random.Next(30, 221), 22f);
				quirrelGO.GetComponent<SpriteRenderer>().flipX = random.Next(0, 2) < 1;
			}
			else quirrelGO.SetActive(false);
			return scene;
		}
	}
}
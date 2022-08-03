using Game.Entities.Shared;
using Game.Managers;
using Game.UI;
using Nawlian.Lib.Systems.Pooling;
using Nawlian.Lib.Utils;
using UnityEngine;

namespace Game.Systems.Run.Rooms
{
	public class BossRoom : CombatRoom
	{
		[SerializeField] protected GameObject _bossPrefab;
		[SerializeField] protected Transform _bossSpawn;

		protected EntityIdentity _bossIdentity;
		protected BossBarUi _bossBar;
		public GameObject Boss => _bossIdentity.gameObject;
		public Vector3 BossSpawnPoint
		{
			get
			{
				if (_bossSpawn)
					return _bossSpawn.position;
				return Vector3.zero;
			}
		}
		public override bool RequiresNavBaking => true;
		public override bool ActivateOnStart => false;
		public virtual bool GiveBossReward => false;
		public virtual bool ActivateOnBossSpawn => true;

		protected override void Awake()
		{
			base.Awake();
			_waveNumber = 1;
		}

		protected override void OnRoomSceneReady()
		{
			base.OnRoomSceneReady();
			_bossIdentity = ObjectPooler.Get(_bossPrefab, BossSpawnPoint, _bossSpawn.rotation, this).GetComponent<EntityIdentity>();

			if (ActivateOnBossSpawn)
				// TODO: wait time according to some animation ?
				Awaiter.WaitAndExecute(1f, Activate);
		}

		protected override void OnActivate()
		{
			_bossBar = GuiManager.OpenMenu<BossBarUi>();
			_bossBar.Bind(_bossIdentity);
		}

		protected override void OnClear()
		{
			base.OnClear();
			if (GiveBossReward)
				GameManager.RewardWithLobbyMoney(Random.Range(RunManager.RunSettings.LobbyMoneyRunReward.x, RunManager.RunSettings.LobbyMoneyRunReward.y + 1));
		}

		public override void OnEnemyKilled(GameObject gameObject) => Clear();
	}
}
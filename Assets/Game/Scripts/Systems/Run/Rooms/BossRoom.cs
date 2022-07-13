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
		[SerializeField] private GameObject _boss;
		[SerializeField] private Transform _bossSpawn;

		private EntityIdentity _bossIdentity;
		private BossBarUi _bossBar;

		public override bool RequiresNavBaking => true;
		public override bool ActivateOnStart => false;
		public virtual bool GiveBossReward => false;
		public virtual bool ActivateOnBossSpawn => true;

		protected override void Awake()
		{
			base.Awake();
			_waveNumber = 1;
		}

		protected override void Start()
		{
			base.Start();
			_bossIdentity = ObjectPooler.Get(_boss, _bossSpawn.position, _bossSpawn.rotation, this).GetComponent<EntityIdentity>();

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
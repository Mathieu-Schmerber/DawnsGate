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

		protected override void Awake()
		{
			base.Awake();
			_waveNumber = 1;
		}

		private void OnEnable()
		{
			RunManager.OnRunEnded += CloseBossUi;
		}

		private void OnDisable()
		{
			RunManager.OnRunEnded -= CloseBossUi;
		}

		protected override void Start()
		{
			_bossIdentity = ObjectPooler.Get(_boss, _bossSpawn.position, _bossSpawn.rotation, this).GetComponent<EntityIdentity>();

			// TODO: wait time according to some animation ?
			Awaiter.WaitAndExecute(1f, Activate);
		}

		private void CloseBossUi() => _bossBar.Close();

		protected override void OnActivate()
		{
			_bossBar = GuiManager.OpenMenu<BossBarUi>();
			_bossBar.Bind(_bossIdentity);
		}

		protected override void OnClear()
		{
			base.OnClear();
			_bossBar.Close();
			GameManager.RewardWithLobbyMoney(Random.Range(RunManager.RunSettings.LobbyMoneyRunReward.x, RunManager.RunSettings.LobbyMoneyRunReward.y + 1));
		}

		public override void OnEnemyKilled(GameObject gameObject) => Clear();
	}
}
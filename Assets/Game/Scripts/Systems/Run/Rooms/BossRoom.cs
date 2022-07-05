using Game.Entities.Shared;
using Game.Managers;
using Game.UI;
using Nawlian.Lib.Systems.Pooling;
using UnityEngine;

namespace Game.Systems.Run.Rooms
{
	public class BossRoom : CombatRoom
	{
		[SerializeField] private GameObject _boss;
		[SerializeField] private Transform _bossSpawn;

		private BossBarUi _bossBar;

		public override bool RequiresNavBaking => true;

		protected override void Awake()
		{
			base.Awake();
			_waveNumber = 1;
		}

		protected override void OnActivate()
		{
			GameObject instance = ObjectPooler.Get(_boss, _bossSpawn.position, _bossSpawn.rotation, this);

			_bossBar = GuiManager.OpenMenu<BossBarUi>();
			_bossBar.Bind(instance.GetComponent<EntityIdentity>());
		}

		public override void OnEnemyKilled(GameObject gameObject)
		{
			_bossBar.Close();
			Clear();
		}
	}
}
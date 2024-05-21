using System;
using System.Collections.Generic;
using System.Linq;
using MuseDotNet.Framework;
namespace AttributeServiceProvider
{
	public enum AttributeType
	{
		MaxHealth,
		Health,
		MaxMana,
		Mana,
		MoveSpeed,
		MoveSpeedCrouched,
		MaxAcceleration,
		JumpZVelocity,
		GravityScale,
		TurnRateYaw,
		MaxStamina,
		Stamina,
		StaminaRegenRate,
		ManaRegenRate,
		DamageBlockCost,
		MaxEnergyArmor,
		EnergyArmor,
		MaxPhysicalArmor,
		PhysicalArmor,
		NewDynamic,
		ItemHeatedMetal,
		MoveSpeedScalar,
		CloakRadius,
		PlayerXP,
		PhysicalArmorXP,
		SwordXP,
		TurretXP,
		ShieldXP,
		SpearXP,
		SpeedXP,
		CloakXP,
		AutoRepairXP,
		FactionXP,
		BowXP,
		ResourceCount,
		Smelting,
		Woodcrafting,
		Woodcutting,
		WoodcuttingTool,
		MaxMining,
		MiningTool,
		Blacksmithing,
		HarvestDurationModifier,
		FurnaceHeat,
		PlayerAttack,
		SpearAttack,
		SpearAttackSpeed,
		SwordAttack,
		SwordAttackSpeed,
		ClipCount,
		MaxAmmo,
		MaxClipSize,
		Ammo,
		ExpectedClipReduction,
		Knockback,
		BuildingDamage,
		CharacterDamage,
		HarvestableDamage,
		Damage,
		DamagePercentIncrease,
		DamagePercentResist,
		FrostDamage,
		MuleDamage,
	}

	public readonly struct AttributeValueModifier(
		AttributeType attributeType,
		float magnitude,
		string additionTag,
		string subtractionTag
	)
	{
		public readonly AttributeType AttributeType = attributeType;
		public readonly float Magnitude = magnitude;
		public readonly string AdditionTag = additionTag;
		public readonly string SubtractionTag = subtractionTag;
	}

	public class AttributeService
	{

		private readonly Dictionary<AttributeType, AttributeValueModifier[]> ModifierRegistry = [];

		private static readonly bool IS_DEBUG_MODE = false;
		private static void Log(string message, LogLevel level = LogLevel.Display)
		{
			if (IS_DEBUG_MODE)
			{
				Debug.Log(level, message);
			}
		}

		public float Get(
			Actor character,
			AttributeType attributeType
		)
		{
			float value = 0.0f;
			foreach (AttributeValueModifier modifier in ModifierRegistry[attributeType])
			{
				int additionCount = character.GetInventoryItemCount(modifier.AdditionTag);
				int subtractionCount = character.GetInventoryItemCount(modifier.SubtractionTag);
				int netCount = additionCount - subtractionCount;
				value += netCount * modifier.Magnitude;
				Log($"get={attributeType}, value={value}, netCount={netCount}");
				if (netCount > 0)
				{
					character.RemoveInventoryItems(modifier.SubtractionTag);
					for (int i = 0; i < additionCount - Math.Abs(netCount); i++)
					{
						character.RemoveInventoryItem(modifier.AdditionTag);
					}
				}
				else if (netCount < 0)
				{
					character.RemoveInventoryItems(modifier.AdditionTag);
					for (int i = 0; i < subtractionCount - Math.Abs(netCount); i++)
					{
						character.RemoveInventoryItem(modifier.SubtractionTag);
					}
				}
			}

			return value;
		}

		public void Set(
			Actor character,
			AttributeType attributeType,
			float value
		)
		{
			float currentValue = Get(character, attributeType);
			Increment(character, attributeType, value - currentValue);
		}

		public float Increment(
			Actor character,
			AttributeType attributeType,
			float delta
		)
		{
			float currentValue = Get(character, attributeType);

			float absProgress = Math.Abs(delta);
			foreach (AttributeValueModifier modifier in ModifierRegistry[attributeType])
			{
				if (absProgress > 0)
				{
					int count = (int)Math.Floor(absProgress / modifier.Magnitude);
					if (count > 0)
					{
						for (int i = 0; i < count; i++)
						{
							if (delta < 0)
							{
								Log($"inc delta={delta}, i={i}/{count}, sub={modifier.Magnitude}");
								character.AddInventoryItem(modifier.SubtractionTag);
							}
							else
							{
								Log($"inc delta={delta}, i={i}/{count}, add={modifier.Magnitude}");
								character.AddInventoryItem(modifier.AdditionTag);
							}
						}
						absProgress -= count * modifier.Magnitude;
					}
				}

			}

			return currentValue + delta;
		}

		public float Multiply(
			Actor character,
			AttributeType attributeType,
			float value
		)
		{
			float product = Get(character, attributeType) * value;
			Set(character, attributeType, product);
			return product;
		}

		public void Init(
			AttributeValueModifier[] modifiers
		)
		{
			List<AttributeType> keys = [];
			foreach (AttributeValueModifier modifier in modifiers)
			{
				if (!keys.Contains(modifier.AttributeType))
				{
					keys.Add(modifier.AttributeType);
				}
			}

			foreach (AttributeType attributeType in keys)
			{
				List<AttributeValueModifier> attributeMods = [];
				foreach (AttributeValueModifier modifier in modifiers)
				{
					if (attributeType == modifier.AttributeType)
					{
						attributeMods.Add(modifier);
					}
				}

				ModifierRegistry[attributeType] = attributeMods.OrderByDescending(modifier => modifier.Magnitude).ToArray();
			}
		}

		// set up as singleton
		private static AttributeService instance;
		public static AttributeService Instance
		{
			get
			{
				instance ??= new AttributeService();
				return instance;
			}
		}
		private AttributeService() { }
	}
}
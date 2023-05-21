using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ACF.Tests
{
	// [ExecuteAlways]
	[RequireComponent(typeof(Image))]
	public class HealthBarTest : MonoBehaviour
	{
		private const string STEP = "_Steps";
		private const string RATIO = "_HSRatio";
		private const string WIDTH = "_Width";
		private const string THICKNESS = "_Thickness";
		
		private static readonly int floatSteps = Shader.PropertyToID(STEP);
		private static readonly int floatRatio = Shader.PropertyToID(RATIO);
		private static readonly int floatWidth = Shader.PropertyToID(WIDTH);
		private static readonly int floatThickness = Shader.PropertyToID(THICKNESS);
		
		[Range(0, 2800f)] public float currentHP = 1000f;
		[Range(0, 2800f)] public float maxHP = 1000f;
		[Range(0, 920f)] public float shield = 0f;
		[Range(0, 10f)] public float speed = 3f;
		
		public float hpShieldRatio;
		public float RectWidth = 100f;
		[Range(0, 5f)]public float Thickness = 2f;
		
		public Image hp;
		public Image damaged;
		public Image sp;
		public Image separator;

		[ContextMenu("Create Material")]
		private void CreateMaterial()
		{
			separator.material = new Material(Shader.Find("UI/Health Separator"));
		}

		private void Start()
		{
			CreateMaterial();		
		}
		

		private void Update()
		{
			if (maxHP < currentHP)
			{
				maxHP = currentHP;
			}

			float step;

			// 쉴드가 존재 할 때
			if (shield > 0)
			{
				// 현재체력 + 쉴드 > 최대 체력
				if (currentHP + shield > maxHP)
				{
					hpShieldRatio = currentHP / (currentHP + shield);
					sp.fillAmount = 1f;
					step = (currentHP) / 500f;
					hp.fillAmount = currentHP / (currentHP + shield);
				}
				else
				{
					sp.fillAmount = (currentHP + shield) / maxHP;
					hpShieldRatio = currentHP / maxHP;
					step = currentHP / 500f;
				
					hp.fillAmount = currentHP / maxHP;
				}
			}
			else
			{
				sp.fillAmount = 0f;
				step = maxHP / 500f;
				hpShieldRatio = 1f;
				
				hp.fillAmount = currentHP / maxHP;
			}
			if (shield <= 0)
				damaged.fillAmount = Mathf.Lerp(damaged.fillAmount, hp.fillAmount, Time.deltaTime * speed);
			else
				damaged.fillAmount = hp.fillAmount;			
			separator.material.SetFloat(floatSteps, step);
			separator.material.SetFloat(floatRatio, hpShieldRatio);
			separator.material.SetFloat(floatWidth, RectWidth);
			separator.material.SetFloat(floatThickness, Thickness);
		}
	}
}
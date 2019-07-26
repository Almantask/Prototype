using System.Collections;
using System.Collections.Generic;
using Project.Core.Characters;
using UnityEngine.UI;
using UnityEngine;

namespace Project.Core.UI {
    public class ResourcesBar : MonoBehaviour {

        public Character _character;
        public RectTransform _nameplate;

        private float _currentHealth;
        public Image healthBarFill;

        public Image actionPoint1;
        public Image actionPoint2;

        private void LateUpdate() {
            Reposition();
        }

        public void SetCharacter(Character character) {
            _character = character;
            _currentHealth = _character.GetAttribute(Characters.Attributes.AttributeType.Health).Value;
            ModifyHealth();
            SetActionPoints(character);
        }

        private void SetActionPoints(Character character) {
            var data = character.Data;
            if(data.ActionsPerTurn == 2) {
                actionPoint1.gameObject.SetActive(true);
                actionPoint2.gameObject.SetActive(true);
            } else if(data.ActionsPerTurn == 1) {
                actionPoint1.gameObject.SetActive(true);
                actionPoint2.gameObject.SetActive(false);
            } else if(data.ActionsPerTurn > 2) {
                actionPoint1.gameObject.SetActive(true);
                actionPoint2.gameObject.SetActive(true);
            } else {
                actionPoint1.gameObject.SetActive(false);
                actionPoint2.gameObject.SetActive(false);
            }
        }

        public void ResetAP(Character character) {
            SetActionPoints(character);
        }

        public void DecreaseAP() {
            if(_character.ActionPointsLeft == 1) {
                actionPoint1.gameObject.SetActive(true);
                actionPoint2.gameObject.SetActive(false);
            } else {
                actionPoint1.gameObject.SetActive(false);
                actionPoint2.gameObject.SetActive(false);
            }
        }

        public void ModifyHealth() {
            float current = _currentHealth;
            _currentHealth = (float)_character.GetAttribute(Characters.Attributes.AttributeType.Health).Value;
            float percent = _currentHealth / current;
            StartCoroutine(ChangeFillAmount(percent));
        }

        private IEnumerator ChangeFillAmount(float percent) {
            var overtime = 0.25f;
            var preChange = healthBarFill.fillAmount;
            var elapsed = 0f;
            while(elapsed < overtime) {
                elapsed += Time.deltaTime;
                healthBarFill.fillAmount = Mathf.Lerp(preChange, percent, elapsed / overtime);
                yield return null;
            }
            healthBarFill.fillAmount = percent;
        }

        private void Reposition() {
            transform.LookAt(Camera.main.transform);
            transform.Rotate(0, 180, 0);
        }
    }
}
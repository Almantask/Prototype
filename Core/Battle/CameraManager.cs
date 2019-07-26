using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {
    public Transform focus;
    private int _followSpeed = 13;

    private bool following;

    private void OnEnable() {
        this.AddObserver(SetFocus, Events.CharacterTurnStarted);
    }

    private void OnDisable() {
        this.RemoveObserver(SetFocus, Events.CharacterTurnStarted);
    }

    private void SetFocus(object sender, object args) {
        focus = ((Events.OnCharacterTurnStarted)args).Character.transform;
        StartCoroutine(FollowTarget());
    }

    private void Update() {
        if(!focus) return;
        if(!following) {
            StartCoroutine(FollowTarget());
        }
    }

    private IEnumerator FollowTarget() {
        following = true;
        while(transform.position != focus.position) {
            transform.position = Vector3.MoveTowards(transform.position, focus.position, Time.deltaTime * _followSpeed);
            yield return 0;
        }
        following = false;
    }
}

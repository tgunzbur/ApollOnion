using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MatType
{
    NONE,
    ALL,
    ONION,
    ONION_HEART,
    ONION_PEELING,
    ONION_PLATE,
    ONION_JUICE,
    ONION_FUEL,
    ONION_PASTE,
    ONION_FIBER,
    ONION_CABLE,
    ONION_GLASS,
    ONION_FABRIC,
    TANK,
    COCKPIT,
    ENGINE,
    ROCKET,
    CLOTHE
}

public class Mat : MonoBehaviour {
    private float speed;

    public virtual MatType getMatType() {
        throw new NotImplementedException();
    }

    public void moveTo(Vector3 target) {
        speed = Time.fixedDeltaTime;
        StartCoroutine(movingAnim(target));
    }

    private IEnumerator movingAnim(Vector3 target) {
        Vector3 startPos = transform.position;
        float maxHeight = startPos.y + Mathf.Abs(startPos.y - target.y) / 2 + 1;
        if (maxHeight < target.y + 0.5f) {
            maxHeight = target.y + 0.5f;
        } else if (maxHeight < startPos.y + 0.5f) {
            maxHeight = startPos.y + 0.5f;
        }

        float denom = (0 - 0.5f) * (0 - 1) * (0.5f - 1);
		float a = (1 * (maxHeight - startPos.y) + 0.5f * (startPos.y - target.y) + 0 * (target.y - maxHeight)) / denom;
		float b = (1 * 1 * (startPos.y - maxHeight) + 0.5f * 0.5f * (target.y - startPos.y) + 0 * 0 * (maxHeight - target.y)) / denom;
		float c = (0.5f * 1 * (0.5f - 1) * startPos.y + 1 * 0 * (1-0) * maxHeight + 0 * 0.5f * (0 - 0.5f) * target.y) / denom;
        float x = 0;
        float step = 0;
        Vector3 newPos = Vector3.zero;
        while (x < 1) {
            if (x == 0) {
                step = 0.01f;
            } else {
                step = Time.deltaTime * speed;
            }
            x += step;
            newPos = transform.position;
            newPos.y = target.y;
            newPos = Vector3.MoveTowards(newPos, target, step);
            newPos.y = a * Mathf.Pow(x, 2) + b * x + c;
            transform.position = newPos;
            yield return new WaitForEndOfFrame();
        }
        if (target.y <= 0) {
            GameObject.Destroy(gameObject);
        } else {
            gameObject.SetActive(false);
        }
    }
}
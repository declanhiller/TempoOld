using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DynamicCamera : MonoBehaviour {
    
    [SerializeField] private Vector2 offset;
    [SerializeField] private List<DynamicCameraTarget> targets;

    [SerializeField] private Camera camera;
    
    [SerializeField] private float maxCameraDistance;
    [SerializeField] private float minCameraDistance;

    [SerializeField] private float speedWhereCameraDoesntFollow = 10f;

    [SerializeField] private float smoothTime = 0.3f;

    [SerializeField] private BoxCollider2D cameraBounds;
    [SerializeField] private float borderLength;

    private Vector3 velocity = Vector3.zero;

    private Vector3 position;
    private bool cameraIsShaking;
    
    private void LateUpdate() {
        float xCoord = 0;
        float yCoord = 0;
        float xMax = targets[0].position.x;
        float xMin = targets[0].position.x;
        float yMax = targets[0].position.y;
        float yMin = targets[0].position.y;
        foreach (DynamicCameraTarget target in targets) {
            if (target.targetType == DynamicCameraTarget.TargetType.PLAYER) {
                if (target.speed > speedWhereCameraDoesntFollow) continue;
                if(target.position.x > cameraBounds.bounds.max.x || target.position.x < cameraBounds.bounds.min.x ||
                   target.position.y > cameraBounds.bounds.max.y || target.position.y < cameraBounds.bounds.min.y) continue;
            }

            xCoord += target.position.x;
            yCoord += target.position.y;
            if (target.position.x > xMax) {
                xMax = target.position.x;
            } else if (target.position.x < xMin) {
                xMin = target.position.x;
            }
            if (target.position.y > yMax) {
                yMax = target.position.y;
            } else if (target.position.y < yMin) {
                yMin = target.position.y;
            }
        }


        Vector2 size = new Vector2(xMax - xMin, yMax - yMin);
        float targetFrustumWidth = size.x + borderLength;
        float targetFrustumHeight = targetFrustumWidth / camera.aspect;
        if(targetFrustumHeight < size.y + borderLength) {
            targetFrustumHeight = size.y + borderLength;
        }

        float cameraDistance = targetFrustumHeight * 0.5f / Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
        if (cameraDistance < minCameraDistance) {
            cameraDistance = minCameraDistance;
        } else if (cameraDistance > maxCameraDistance) {
            cameraDistance = maxCameraDistance;
        }
            
        xCoord = xCoord / targets.Count;
        yCoord = yCoord / targets.Count;

        
        var frustumHeight = 2.0f * cameraDistance * Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
        var frustumWidth = frustumHeight * camera.aspect;
        
        
        float top = yCoord + frustumHeight / 2;
        float bottom = yCoord - frustumHeight / 2;
        float left = xCoord - frustumWidth / 2;
        float right = xCoord + frustumWidth / 2;
        
        Vector2 frustumPlaneMax = new Vector2(right, top);
        Vector2 frustumPlaneMin = new Vector2(left, bottom);

        Vector2 boundsMax = cameraBounds.bounds.max;
        Vector2 boundsMin = cameraBounds.bounds.min;

        if (frustumPlaneMax.x > boundsMax.x) {
            xCoord = boundsMax.x - frustumWidth / 2;
        } else if (frustumPlaneMin.x < boundsMin.x) {
            xCoord = boundsMin.x + frustumWidth / 2;
        }

        
        if (frustumPlaneMax.y > boundsMax.y) {
            yCoord = boundsMax.y - frustumHeight / 2;
        } else if (frustumPlaneMin.y < boundsMin.y) {
            yCoord = boundsMin.y + frustumHeight / 2;
        }

        Vector3 targetPosition = new Vector3(xCoord, yCoord, -cameraDistance) + (Vector3) offset;

        Vector3 smoothDampPosition = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, 0.3f);

        position = smoothDampPosition;
        if (!cameraIsShaking)
        {
            transform.position = smoothDampPosition;
        }
    }

    [SerializeField] private float shakeDurationWhenHit;
    [SerializeField] private float shakeMagnitudeWhenHit;
    [SerializeField] private float shakeDurationWhenDeath;
    [SerializeField] private float shakeMagnitudeWhenDeath;



    public void PlayPlayerHitShake()
    {
        StartCoroutine(Shake(shakeDurationWhenHit, shakeMagnitudeWhenHit));
    }

    public void PlayPlayerDieShake()
    {
        StartCoroutine(Shake(shakeDurationWhenDeath, shakeMagnitudeWhenDeath));
    }
    
    private IEnumerator Shake(float duration, float magnitude)
    {
        cameraIsShaking = true;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            Vector3 cameraPos = position + Random.insideUnitSphere * magnitude;
            transform.position = cameraPos;
            elapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        
        cameraIsShaking = false;
    }

    public void AddTarget(DynamicCameraTarget target) {
        targets.Add(target);
    }

    public void RemoveTarget(DynamicCameraTarget target) {
        targets.Remove(target);
    }

    private void OnDrawGizmos() {
        var frustumHeight = 2.0f * Math.Abs(camera.transform.position.z) * Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
        var frustumWidth = frustumHeight * camera.aspect;
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector3(transform.position.x, transform.position.y), new Vector3(frustumWidth, frustumHeight));
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(cameraBounds.offset, cameraBounds.size);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliceController : MonoBehaviour
{
    private static readonly List<PoliceController> activePolice = new List<PoliceController>();

    [Header("Chase")]
    [SerializeField] private float chaseSpeed = 8f;
    [SerializeField] private float catchDistance = 0.15f;
    [SerializeField] private bool moveOnlyByX = true;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string runBoolName = "IsRunning";

    [Header("Flip")]
    [SerializeField] private Transform visualRootToFlip;
    [SerializeField] private bool positiveScaleXFacingRight = true;

    private Coroutine chaseCoroutine;

    private void OnEnable()
    {
        if (!activePolice.Contains(this))
            activePolice.Add(this);
    }

    private void OnDisable()
    {
        activePolice.Remove(this);
    }

    public static bool StartClosestChase(Transform target, Action onCaught)
    {
        activePolice.RemoveAll(item => item == null || !item.isActiveAndEnabled);

        if (target == null || activePolice.Count == 0)
            return false;

        PoliceController closest = null;
        float bestDistance = float.MaxValue;

        for (int i = 0; i < activePolice.Count; i++)
        {
            float distance = Mathf.Abs(activePolice[i].transform.position.x - target.position.x);
            if (distance < bestDistance)
            {
                bestDistance = distance;
                closest = activePolice[i];
            }
        }

        if (closest == null)
            return false;

        closest.StartChase(target, onCaught);
        return true;
    }

    public void StartChase(Transform target, Action onCaught)
    {
        PolicePatrol patrol = GetComponent<PolicePatrol>();
        if (patrol != null)
            patrol.StopPatrol();

        if (chaseCoroutine != null)
            StopCoroutine(chaseCoroutine);

        chaseCoroutine = StartCoroutine(ChaseRoutine(target, onCaught));
    }

    private IEnumerator ChaseRoutine(Transform target, Action onCaught)
    {
        SetRunning(true);

        while (target != null)
        {
            Vector3 pos = transform.position;
            Vector3 targetPos = target.position;

            float dx = targetPos.x - pos.x;
            FaceByDirection(dx);

            if (moveOnlyByX)
            {
                float newX = Mathf.MoveTowards(pos.x, targetPos.x, chaseSpeed * Time.deltaTime);
                transform.position = new Vector3(newX, pos.y, pos.z);

                if (Mathf.Abs(targetPos.x - transform.position.x) <= catchDistance)
                    break;
            }
            else
            {
                transform.position = Vector3.MoveTowards(pos, targetPos, chaseSpeed * Time.deltaTime);

                if (Vector3.Distance(transform.position, targetPos) <= catchDistance)
                    break;
            }

            yield return null;
        }

        SetRunning(false);
        onCaught?.Invoke();
    }

    private void SetRunning(bool value)
    {
        if (animator != null && !string.IsNullOrEmpty(runBoolName))
            animator.SetBool(runBoolName, value);
    }

    private void FaceByDirection(float directionX)
    {
        if (visualRootToFlip == null)
            return;

        if (Mathf.Abs(directionX) < 0.001f)
            return;

        bool shouldFaceRight = directionX > 0f;
        Vector3 scale = visualRootToFlip.localScale;
        float absX = Mathf.Max(0.0001f, Mathf.Abs(scale.x));

        bool positiveNeeded = positiveScaleXFacingRight ? shouldFaceRight : !shouldFaceRight;
        scale.x = positiveNeeded ? absX : -absX;
        visualRootToFlip.localScale = scale;
    }
}

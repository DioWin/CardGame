using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class CardExplosionEffect : MonoBehaviour
{
    [System.Serializable]
    private class FragmentData
    {
        public Rigidbody2D rb;
        [HideInInspector] public Vector3 startPos;
        [HideInInspector] public Quaternion startRot;
    }

    [SerializeField] private List<Rigidbody2D> fragments = new List<Rigidbody2D>();
    [SerializeField] private List<Image> icons = new List<Image>();

    [SerializeField] private float explosionForce = 300f;
    [SerializeField] private float torqueForce = 200f;
    [SerializeField] private float durationBeforeFade = 0.5f;
    [SerializeField] private float fadeDuration = 0.5f;

    [SerializeField] private GameObject view;

    private List<FragmentData> fragmentDataList = new List<FragmentData>();
    private bool exploded = false;

    private void Awake()
    {
        foreach (var rb in fragments)
        {
            var data = new FragmentData
            {
                rb = rb,
                startPos = rb.transform.localPosition,
                startRot = rb.transform.localRotation,
            };
            fragmentDataList.Add(data);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!exploded)
            {
                Explode();
                exploded = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (exploded)
            {
                ResetFragments();
                exploded = false;
            }
        }
    }

    public void Init(Sprite icon, Transform perent)
    {
        view.transform.SetParent(perent);

        foreach (var item in icons)
        {
            item.sprite = icon;
        }
    }

    public void Explode()
    {
        view.SetActive(true);


        foreach (var data in fragmentDataList)
        {
            var rb = data.rb;

            rb.simulated = true;
            Vector2 randomDir = Random.insideUnitCircle.normalized;
            float randomForce = Random.Range(explosionForce * 0.8f, explosionForce * 1.2f);
            float randomTorque = Random.Range(-torqueForce, torqueForce);

            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0;
            rb.AddForce(randomDir * randomForce);
            rb.AddTorque(randomTorque);
            var sr = rb.GetComponent<CanvasGroup>();
            if (sr != null)
            {
                sr.DOFade(0f, fadeDuration)
                  .SetDelay(durationBeforeFade)
                  .SetEase(Ease.InOutQuad).OnComplete(() =>
                  {
                      Destroy(view.gameObject);
                  });
            }
        }
    }

    public void ResetFragments()
    {
        foreach (var data in fragmentDataList)
        {
            var rb = data.rb;
            rb.simulated = false;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0;

            rb.transform.localPosition = data.startPos;
            rb.transform.localRotation = data.startRot;

            var sr = rb.GetComponent<CanvasGroup>();
            if (sr != null)
            {
                sr.alpha = 1f;
            }
        }
    }
}

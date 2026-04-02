using UnityEngine;

public class FogManager : MonoBehaviour, IStageClearHandler
{
    [SerializeField] private bool stage1Clear;
    [SerializeField] private GameObject stage2Fog;
    [SerializeField] private bool stage2Clear;
    [SerializeField] private GameObject stage3Fog;

    private void OnEnable()
    {
        Subject<IStageClearHandler>.Attach(this);
    }

    private void OnDisable()
    {
        Subject<IStageClearHandler>.Detach(this);
    }

    public void OnStageClear(int stage)
    {
        switch (stage)
        {
            case 1:
                stage1Clear = true;
                break;
            case 2:
                stage2Clear = true;
                break;
        }

        ChangeFogState();
    }

    private void ChangeFogState()
    {
        if (stage1Clear)
            stage2Fog.SetActive(false);

        if (stage2Clear)
            stage3Fog.SetActive(false);
    }
}
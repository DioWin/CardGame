using UnityEngine;

public class GameCardView : BaseCardView
{
    private void Update()
    {
        if (!isInitialized)
            return;

        if (isDragging)
        {
            FollowCursor();
            HandleRotationToMouse();
        }
        else
        {
            visual.position = Vector3.Lerp(
                visual.position,
                modelRect.position,
                Time.deltaTime * followDefaultPosSpeed
            );

            if (!nudgeInProgress)
                LerpToRotation(modelRect.eulerAngles.z, 10f);
        }
    }
}

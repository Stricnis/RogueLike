using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour {

    public GameObject gameManager;
    public Player player;
    public BoardManager boardScript;
    public Vector3 offset;
    private float _startOrthographicSize;
    public float _runtimeOrthographicSize = 0f;

    public float zoomDuration = 1.0f;
    private float zoomElapsed = 0.0f;
    public float moveDuration = 2.0f;
    private float moveElapsed = 0.0f;
    void Awake () {
        if (GameManager.instance == null)
            Instantiate(gameManager);

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        boardScript = GameManager.instance.boardScript;

        Vector3 v3T = transform.position;
        v3T.x = (float)(boardScript.columns - 1f) / 2f;
        v3T.y = (float)(boardScript.rows - 1f) / 2f;
        transform.position = v3T;

        // Zoom out at the beginning of the level to see where enemies are
        _runtimeOrthographicSize = Camera.main.orthographicSize;
        _startOrthographicSize = (float)boardScript.columns / 2f + 1;
        Camera.main.orthographicSize = _startOrthographicSize;
    }

    private void LateUpdate()
    {
        if (player.HasMovedThisTurn == true)
        {
            // Zoom in for a better view
            if (Camera.main.orthographicSize - _runtimeOrthographicSize > float.Epsilon)
            {
                zoomElapsed += Time.deltaTime / zoomDuration;
                Camera.main.orthographicSize = Mathf.Lerp(_startOrthographicSize, _runtimeOrthographicSize, zoomElapsed);
                //this next line i'm not sure of, I'm not familiar with CameraMovement.ypos
                //Camera.main.GetComponent<CameraMovement>().ypos = Mathf.Lerp(ypos1, ypos2, elapsed);
            }

            // Follow the player but not when he's too close to the edge
            moveElapsed += Time.deltaTime / moveDuration;
            Vector3 camPos = transform.position;
            Vector3 targetPos = player.transform.position;

            float halfSize = Camera.main.orthographicSize / 2;
            if (targetPos.x < halfSize)
                targetPos.x = halfSize;
            else if (targetPos.x > boardScript.columns - halfSize)
                targetPos.x = boardScript.columns - halfSize;

            if (targetPos.y < halfSize)
                targetPos.y = halfSize;
            else if (targetPos.y > boardScript.rows - halfSize)
                targetPos.y = boardScript.rows - halfSize;

            camPos.x = Mathf.Lerp(camPos.x, targetPos.x, moveElapsed);
            camPos.y = Mathf.Lerp(camPos.y, targetPos.y, moveElapsed);

            transform.position = camPos;
        }
    }
}

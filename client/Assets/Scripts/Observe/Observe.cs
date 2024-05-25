using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Observe : MonoBehaviour
{
    public float RotateSpeed;
    public float MoveSpeed;
    public float FreeMoveSpeed;
    public const float FreeMaxPitch = 80;
    public enum CameraStatus {freeCamera=0, player};
    public CameraStatus cameraStatus {get; private set;}
    public Player _target;
    private List<Player> _players;
    public UnityEngine.Transform initialTransform;
    public int PlayerNumber {get; private set; }
    Vector3 offset;
    public float rotationSpeed;
    public Vector3 velocity = Vector3.zero;
    public Vector3 lookatPositionvelocity = Vector3.zero;
    public Vector3 lookatPosition;

    void Start()
    {
        offset = new Vector3(5, 5, 5);
        initialTransform = transform;
        _players = new();
        RotateSpeed = 100f;
        rotationSpeed = 75f;
        MoveSpeed = 0.1f;
        FreeMoveSpeed = 10f;
        cameraStatus = CameraStatus.freeCamera;
        _target = null;
    }

    void Update()
    {
        if (cameraStatus == CameraStatus.player)
        {
            Rotate();
            Rollup();
            ExchangeStatus();
            Follow();
        }
        else
        {
            Move();
            ExchangeStatus();
        }
    }
    void visualAngleReset(Vector3 from, Vector3 to)
    {
        offset = (from - to) * 8 / (from - to).magnitude;
    }

    void ExchangeStatus()
    {
        if (Input.GetMouseButtonDown(0))
        {

            PointerEventData pointerEventData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };
            List<RaycastResult> results = new List<RaycastResult>();

            EventSystem.current.RaycastAll(pointerEventData, results);

            foreach (RaycastResult result in results)
            {
                if (result.gameObject.GetComponent<Button>() != null)
                {
                    return;
                }
            }

            Dictionary<int ,Player> dict= PlayerSource.GetPlayers();
            _players.Clear();
            foreach (KeyValuePair<int, Player> player in dict)
            {
                _players.Add(player.Value);
            } 
            if(cameraStatus == CameraStatus.player)
            {
                // Retry target
                if (_players.Count - 1 > PlayerNumber)
                {
                    PlayerNumber++;
                    _target = _players[PlayerNumber];
                    Debug.Log(transform.position);
                    Debug.Log($"target {_target.playerObj.transform.position}");
                    visualAngleReset(transform.position, GetHeadPos(_target.playerObj.transform.position));
                    Debug.Log($"after {transform.position}");
                }
                else
                {
                    PlayerNumber = -1;
                    cameraStatus = CameraStatus.freeCamera;
                }

            }
            else if(cameraStatus == CameraStatus.freeCamera && _players.Count != 0)
            {
                PlayerNumber++;
                cameraStatus = CameraStatus.player;
                _target = _players[PlayerNumber];
                //Vector3 newOffset = GetHeadPos(_target.playerObj.transform.position) - transform.position;
                //newOffset *= 8 / newOffset.magnitude;
                Debug.Log(_target == null ? "Target is null" : "Target is not null");
                visualAngleReset(transform.position, GetHeadPos(_target.playerObj.transform.position));
            }
        }
    }
    public float zoomSpeed = 1f;
    float zoom;
    void Follow()
    {
        zoom = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        if (zoom != 0)
        {
            offset -= zoom * offset;
        }
        //��ͷ����
       Vector3 headPosition = GetHeadPos(new Vector3(_target.PlayerPosition.x, 0f, _target.PlayerPosition.y));
        transform.position = Vector3.SmoothDamp(transform.position, headPosition + offset, ref velocity, MoveSpeed);
        lookatPosition = Vector3.SmoothDamp(lookatPosition, headPosition, ref lookatPositionvelocity, MoveSpeed);
        transform.LookAt(lookatPosition);
        // transform.position = GetHeadPos(_target.playerObj.transform.position) - offset;
    }


    public bool isRotating, lookup;
    float mousex, mousey;

    Vector3 GetHeadPos(Vector3 playerPos)
    {
        return new Vector3(playerPos.x, playerPos.y + 1.5f, playerPos.z);
    }
    void Rotate()
    {
        isRotating = true;
        if (isRotating)
        {
            mousex = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;

            transform.RotateAround(GetHeadPos(_target.playerObj.transform.position), Vector3.up, mousex);
            offset = Quaternion.AngleAxis(mousex, Vector3.up) * offset;
            //offset = GetHeadPos(_target.playerObj.transform.position) - transform.position;
        }
    }
    void Rollup()
    {
        lookup = true;
        if (lookup)
        {
            mousey = -Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

            if (Mathf.Abs(transform.rotation.x + mousey-initialTransform.rotation.x) > 90) mousey = 0;
            transform.RotateAround(GetHeadPos(_target.playerObj.transform.position), transform.right, mousey);
            offset = Quaternion.AngleAxis(mousey, transform.right) * offset;
        }

    }
    void Move()
    {
        CameraRotate();
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        // Move when "w a s d" is pressed
        if (Mathf.Abs(vertical) > 0.01)
        {
            Vector3 fowardVector = transform.forward;
            fowardVector = new Vector3(fowardVector.x, 0, fowardVector.z).normalized;
            // move forward
            transform.Translate(FreeMoveSpeed * Time.deltaTime * vertical * fowardVector, Space.World);
        }
        if (Mathf.Abs(horizontal) > 0.01)
        {
            Vector3 rightVector = transform.right;
            rightVector = new Vector3(rightVector.x, 0, rightVector.z).normalized;
            // move aside 
            transform.Translate(FreeMoveSpeed * Time.deltaTime * horizontal * rightVector, Space.World);
        }

        // Fly up if space is clicked
        if (Input.GetKey(KeyCode.Space))
        {
            transform.Translate(FreeMoveSpeed * Time.deltaTime * Vector3.up, Space.World);
        }
        // Fly down if left shift is clicked
        if (Input.GetKey(KeyCode.LeftShift))
        {
            transform.Translate(FreeMoveSpeed * Time.deltaTime * Vector3.down, Space.World);
        }

    }
    void CameraRotate()
    {
        if (Input.GetMouseButton(1))
        {
            float MouseX = Input.GetAxis("Mouse X");
            float MouseY = Input.GetAxis("Mouse Y");

            if ((Mathf.Abs(MouseX) > 0.01 || Mathf.Abs(MouseY) > 0.01))
            {
                transform.Rotate(new Vector3(0, MouseX * RotateSpeed * Time.deltaTime, 0), Space.World);

                float rotatedPitch = transform.eulerAngles.x - MouseY * RotateSpeed * Time.deltaTime * 1f;
                if (Mathf.Abs(rotatedPitch > 180 ? 360 - rotatedPitch : rotatedPitch) < FreeMaxPitch)
                {
                    transform.Rotate(new Vector3(-MouseY * RotateSpeed * Time.deltaTime * 1f, 0, 0));
                }
                else
                {
                    if (transform.eulerAngles.x < 180)
                        transform.eulerAngles = new Vector3((FreeMaxPitch - 1e-6f), transform.eulerAngles.y, 0);
                    else
                        transform.eulerAngles = new Vector3(-(FreeMaxPitch - 1e-6f), transform.eulerAngles.y, 0);
                }
            }
        }
    }
}

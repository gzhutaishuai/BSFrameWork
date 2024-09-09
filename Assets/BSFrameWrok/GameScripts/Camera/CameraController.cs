using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    #region  WASD
    private float _panSpeed;//相机平移速度
    [SerializeField] private float _moveTime;//过渡插值
    [SerializeField] private float _normalSpeed;//不按shift
    [SerializeField] private float _fastSpeed;//按住shift
    private Vector3 _newPos;
    #endregion
    #region Rotation
    private Quaternion _newRotation;
    [SerializeField] private float _rotationspeed;
    #endregion
    #region Scale
    private Transform _mainCamera;//子物体相机
    private Vector3 _newZoom;
    [SerializeField] private Vector3 _zoomAmount;//改变Y,Z的值
    #endregion
    #region Limit
    [Header("平移范围")]
    [SerializeField] private float _minX;
    [SerializeField] private float _maxX;
    [SerializeField] private float _minY;
    [SerializeField] private float _maxY;
    [SerializeField] private float _minZ;
    [SerializeField] private float _maxZ;
    [Header("缩放范围")]
    [SerializeField] private float _scaleMinY;
    [SerializeField] private float _scaleMaxY;
    [SerializeField] private float _scaleMinZ;
    [SerializeField] private float _scaleMaxZ;
    #endregion
    #region MouseControll
    private Vector3 _dragStartPos;
    private Vector3 _dragCurPos;
    #endregion
    private void Start()
    {
        _newPos= transform.position; 
        _newRotation= transform.rotation;
        _mainCamera=transform.GetChild(0);
        _newZoom = _mainCamera.localPosition;
    }

    private void Update()
    {
        
    }
    private void LateUpdate()
    {
        HandleMoveController();//通过键盘控制摄像机
        //MouseMoveController();//通过鼠标控制摄像机
    }

    private void MouseMoveController()
    {
        if(Input.GetMouseButtonDown(1))
        {
            Plane plane=new Plane(Vector3.up,Vector3.zero);
            Ray ray=Camera.main.ScreenPointToRay(Input.mousePosition);
            float distance;
            if(plane.Raycast(ray, out distance))
            {
                _dragStartPos=ray.GetPoint(distance);
            }
        }    

        if(Input.GetMouseButton(1))
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float distance;
            if (plane.Raycast(ray, out distance))
            {
                _dragCurPos = ray.GetPoint(distance);

                Vector3 difference=_dragStartPos-_dragCurPos;
                _newPos=transform.position+difference;
            }
        }
        _newZoom+=Input.mouseScrollDelta.y*_zoomAmount*_moveTime;
    }

    private void HandleMoveController()
    {
        if(Input.GetKey(KeyCode.LeftShift))
        {
            _panSpeed=_fastSpeed;
        }
        else
        {
            _panSpeed=_normalSpeed;
        }

        if (Input.GetKey(KeyCode.UpArrow)||Input.GetKey(KeyCode.W))
        {
            _newPos += transform.forward * _panSpeed * Time.deltaTime;//Z轴 向前移动
        }
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            _newPos -= transform.forward * _panSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            _newPos -= transform.right * _panSpeed * Time.deltaTime;//X轴，向左移动
        }
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            _newPos += transform.right * _panSpeed * Time.deltaTime;
        }

        if(Input.GetKey(KeyCode.E))
        {
            _newRotation *= Quaternion.Euler(Vector3.up * _rotationspeed);//Q;逆时针
        }
        if (Input.GetKey(KeyCode.Q))
        {
            _newRotation *= Quaternion.Euler(Vector3.down * _rotationspeed);//E;顺时针
        }

        //缩放和变大
        if(Input.GetKey(KeyCode.R))
        {
            _newZoom += _zoomAmount;
        }
        if (Input.GetKey(KeyCode.F))
        {
            _newZoom -= _zoomAmount;
        }

        _newPos.x = Mathf.Clamp(_newPos.x, _minX, _maxX);
        _newPos.z = Mathf.Clamp(_newPos.z, _minZ, _maxZ);

        _newZoom.y = Mathf.Clamp(_newZoom.y, _scaleMinY, _scaleMaxY);
        _newZoom.z = Mathf.Clamp(_newZoom.z, _scaleMinZ, _scaleMaxZ);

        transform.position=Vector3.Lerp(transform.position, _newPos, _moveTime*Time.deltaTime);
        transform.rotation=Quaternion.Lerp(transform.rotation,_newRotation, _moveTime*Time.deltaTime);
        _mainCamera.transform.localPosition=_newZoom;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;

public abstract class BaseSkill : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [Header("UI")]
    [SerializeField] protected Image _image;
    protected bool isPointerDown = false;
    protected Vector3 _offset;
    protected Vector3 startPosition;
    protected bool canDrag = true;
    protected Tween tweenMove;
    protected Tween tweenScale;

    protected abstract void OnPointerDownAction();
    protected abstract void OnPointerUpAction();
    protected abstract void OnDragAction();
    protected abstract void OpenShopToBuySkill();
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!GameManager.Instance.isLose && !canDrag)
        {
            OpenShopToBuySkill();
            return;
        }
        if (!canDrag) return;
        if (GameManager.Instance.isLose) 
        {
            canDrag = false;
            ReturnToBasePosition();
            return;
        }
        isPointerDown = true;
        if (tweenMove != null)
        {
            tweenMove.Kill();
        }
        _offset = transform.position - GameManager.Instance.GetMousePosition();
        GameManager.Instance.EnableSkill(true);
        _image.transform.DOScale(0f, 0.1f);
        tweenScale = transform.DOScale(1.2f, 0.1f);
        OnPointerDownAction();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!canDrag) return;
        if (GameManager.Instance.isLose) 
        {
            canDrag = false;
            ReturnToBasePosition();
            return;
        }
        isPointerDown = false;
        GameManager.Instance.EnableSkill(false);
        OnPointerUpAction();
        _image.transform.DOScale(1f, 0.1f);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!canDrag) return;
        if (GameManager.Instance.isLose) 
        {
            canDrag = false;
            ReturnToBasePosition();
            return;
        }
        if (tweenMove != null)
        {
            tweenMove.Kill();
        }
        transform.position = GameManager.Instance.GetMousePosition() + _offset;
        OnDragAction();
    }

    protected void ReturnToBasePosition()
    {
        if (tweenMove != null)
        {
            tweenMove.Kill();
        }
        if (tweenScale != null)
        {
            tweenScale.Kill();
        }
        tweenMove = transform.DOMove(startPosition, 0.2f);
        tweenScale = transform.DOScale(1f, 0.2f);
    }

    public void SetCanUseSkill(bool isCanUse)
    {
        canDrag = isCanUse;
        UpdateTickImage();
    }

    private void UpdateTickImage ()
    {
        ResourcesHolder resourcesHolder = GameManager.Instance.GetResourcesHolder();
        if (canDrag)
        {
            _image.sprite = resourcesHolder.tickSprites[1];
        }
        else
        {
            _image.sprite = resourcesHolder.tickSprites[0];
        }
    }
}

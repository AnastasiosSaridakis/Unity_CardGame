using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TabNavigator : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            EventSystem system = EventSystem.current;
            GameObject curObj = system.currentSelectedGameObject;
            GameObject nextObj = null;
            if (!curObj)
            {
                nextObj = system.firstSelectedGameObject;
            }
            else
            {
                Selectable curSelect = curObj.GetComponent<Selectable>();
                Selectable nextSelect =
                    Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)     
                        ? curSelect.FindSelectableOnUp()
                        : curSelect.FindSelectableOnDown();
                if (nextSelect)
                {
                    nextObj = nextSelect.gameObject;
                }
            }

            if (nextObj)
            {
                system.SetSelectedGameObject(nextObj, new BaseEventData(system));
            }
        }

    }
}

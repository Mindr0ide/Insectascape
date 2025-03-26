using System;
using UnityEngine;

namespace Scenes.Scripts
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private float speed;
        private float currentPosX;
        private Vector3 velocity = Vector3.zero;

        [SerializeField] private Transform player;
        [SerializeField] private float aheadDistance;
        [SerializeField] private float cameraSpeed;

        private float lookAhead;
        

        private void Update()
        {
            // Room Camera movement
            // transform.position = Vector3.SmoothDamp(new Vector3(transform.position.x, transform.position.y, transform.position.z), velocity, ref velocity, speed * Time.deltaTime);
            
            transform.position = new Vector3(player.position.x + lookAhead, player.position.y, transform.position.z);
            lookAhead = Mathf.Lerp(lookAhead, (aheadDistance * player.localScale.x), Time.deltaTime * cameraSpeed);
        }

        public void MoveToNewRoom(Transform newRoom)
        {
            currentPosX = newRoom.position.x;
        }
    }
}
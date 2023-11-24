using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingObject : MonoBehaviour
{
        public float amplitude = 0.2f;
        public float frequency = 0.1f;
     
        // Position Storage Variables
        private Vector2 tempPos = new Vector2();
        
        void Update () {
                // Float up/down with a Sin()
                
                tempPos = transform.position;
                tempPos.y += Mathf.Sin (Time.fixedTime * Mathf.PI * frequency) * amplitude;
 
                transform.position = tempPos;
        }
}

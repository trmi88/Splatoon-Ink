using UnityEngine;

public class CollisionPainter : MonoBehaviour{
    public Color paintColor;
    
    public float radius = 1;
    public float strength = 1;
    public float hardness = 1;

    private void OnCollisionStay(Collision other) {
        Paintable p = other.collider.GetComponent<Paintable>();
        if(p != null){
            Debug.Log("Collision Stay");
            Vector3 pos = other.contacts[0].point;
            Debug.Log("contact0 = " + pos);
            PaintManager.instance.paint(p, pos, radius, hardness, strength, paintColor);
        }
    }
}

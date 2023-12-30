using Dojo;
using UnityEngine;

public class InitEntities : MonoBehaviour
{
    public WorldManager worldManager;
    // Start is called before the first frame update
    void Start()
    {
        worldManager.synchronizationMaster.OnEntitySpawned += InitEntity;
        foreach (var entity in worldManager.Entities())
        {
            InitEntity(entity);
        }
    }

    private void InitEntity(GameObject entity)
    {
        // var capsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        // // change color of capsule to a random color
        // capsule.GetComponent<Renderer>().material.color = Random.ColorHSV();
        // capsule.transform.parent = entity.transform;

        // // create a new GameObject for the text
        // GameObject textObject = new GameObject("TextTag");
        // textObject.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        // textObject.transform.parent = capsule.transform;
        // textObject.transform.localPosition = new Vector3(-1, 2, 0);

        // // add a Text component to the new GameObject
        // var textTag = textObject.AddComponent<TextMesh>();

        // // set the properties of the Text component
        // textTag.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        // textTag.color = Color.black;

        // add text to the position component
        // var position = entity.GetComponent<Position>();
        // position.textTag = textTag;
    }
}

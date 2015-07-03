using UnityEngine;

// Note: TypedMonoBehaviour and ObservableMonoBehaviour cause some performance down.
// I don't recommend instantiate many Typed/ObservableMonoBehaviour.
// If you want to observe MonoBehaviour's event, copy from ObservableMonoBehaviour and paste to your simple MonoBehaviour.

/// <summary>
/// If you want to use coroutine, implements like "new protected IEnumerator OnMouseDown() { }".
/// </summary>
public class TypedMonoBehaviour : MonoBehaviour
{
    /// <summary>Awake is called when the script instance is being loaded.</summary>
    protected virtual void Awake() { }

    /// <summary>This function is called every fixed framerate frame, if the MonoBehaviour is enabled.</summary>
    protected virtual void FixedUpdate() { }

    /// <summary>LateUpdate is called every frame, if the Behaviour is enabled.</summary>
    protected virtual void LateUpdate() { }

    /// <summary>Callback for setting up animation IK (inverse kinematics).</summary>
    protected virtual void OnAnimatorIK(int layerIndex) { }

    /// <summary>Callback for processing animation movements for modifying root motion.</summary>
    protected virtual void OnAnimatorMove() { }

    /// <summary>Sent to all game objects when the player gets or loses focus.</summary>
    protected virtual void OnApplicationFocus(bool focus) { }

    /// <summary>Sent to all game objects when the player pauses.</summary>
    protected virtual void OnApplicationPause(bool pause) { }

    /// <summary>Sent to all game objects before the application is quit.</summary>
    protected virtual void OnApplicationQuit() { }

    /// <summary>If OnAudioFilterRead is implemented, Unity will insert a custom filter into the audio DSP chain.</summary>
    protected virtual void OnAudioFilterRead(float[] data, int channels) { }

    /// <summary>OnBecameInvisible is called when the renderer is no longer visible by any camera.</summary>
    protected virtual void OnBecameInvisible() { }

    /// <summary>OnBecameVisible is called when the renderer became visible by any camera.</summary>
    protected virtual void OnBecameVisible() { }

    /// <summary>OnCollisionEnter is called when this collider/rigidbody has begun touching another rigidbody/collider.</summary>
    protected virtual void OnCollisionEnter(Collision collision) { }

    /// <summary>Sent when an incoming collider makes contact with this object's collider (2D physics only).</summary>
    protected virtual void OnCollisionEnter2D(Collision2D coll) { }

    /// <summary>OnCollisionExit is called when this collider/rigidbody has stopped touching another rigidbody/collider.</summary>
    protected virtual void OnCollisionExit(Collision collisionInfo) { }

    /// <summary>Sent when a collider on another object stops touching this object's collider (2D physics only).</summary>
    protected virtual void OnCollisionExit2D(Collision2D coll) { }

    /// <summary>OnCollisionStay is called once per frame for every collider/rigidbody that is touching rigidbody/collider.</summary>
    protected virtual void OnCollisionStay(Collision collisionInfo) { }

    /// <summary>Sent each frame where a collider on another object is touching this object's collider (2D physics only).</summary>
    protected virtual void OnCollisionStay2D(Collision2D coll) { }

    /// <summary>Called on the client when you have successfully connected to a server.</summary>
    protected virtual void OnConnectedToServer() { }

    /// <summary>OnControllerColliderHit is called when the controller hits a collider while performing a Move.</summary>
    protected virtual void OnControllerColliderHit(ControllerColliderHit hit) { }

    /// <summary>This function is called when the MonoBehaviour will be destroyed.</summary>
    protected virtual void OnDestroy() { }

    /// <summary>This function is called when the behaviour becomes disabled () or inactive.</summary>
    protected virtual void OnDisable() { }

    /// <summary>Implement OnDrawGizmos if you want to draw gizmos that are also pickable and always drawn.</summary>
    protected virtual void OnDrawGizmos() { }

    /// <summary>Implement this OnDrawGizmosSelected if you want to draw gizmos only if the object is selected.</summary>
    protected virtual void OnDrawGizmosSelected() { }

    /// <summary>This function is called when the object becomes enabled and active.</summary>
    protected virtual void OnEnable() { }

#if FALSE // OnGUI called multiple time per frame update and it cause performance issue, If you want to need OnGUI, copy & paste this code on your MonoBehaviour

    /// <summary>OnGUI is called for rendering and handling GUI events.</summary>
    protected virtual void OnGUI() { }

#endif

    /// <summary>Called when a joint attached to the same game object broke.</summary>
    protected virtual void OnJointBreak(float breakForce) { }

    /// <summary>This function is called after a new level was loaded.</summary>
    protected virtual void OnLevelWasLoaded(int level) { }

#if !(UNITY_IPHONE || UNITY_ANDROID)

    /// <summary>OnMouseDown is called when the user has pressed the mouse button while over the GUIElement or Collider.</summary>
    protected virtual void OnMouseDown() { }

    /// <summary>OnMouseDrag is called when the user has clicked on a GUIElement or Collider and is still holding down the mouse.</summary>
    protected virtual void OnMouseDrag() { }

    /// <summary>OnMouseEnter is called when the mouse entered the GUIElement or Collider.</summary>
    protected virtual void OnMouseEnter() { }

    /// <summary>OnMouseExit is called when the mouse is not any longer over the GUIElement or Collider.</summary>
    protected virtual void OnMouseExit() { }

    /// <summary>OnMouseOver is called every frame while the mouse is over the GUIElement or Collider.</summary>
    protected virtual void OnMouseOver() { }

    /// <summary>OnMouseUp is called when the user has released the mouse button.</summary>
    protected virtual void OnMouseUp() { }

    /// <summary>OnMouseUpAsButton is only called when the mouse is released over the same GUIElement or Collider as it was pressed.</summary>
    protected virtual void OnMouseUpAsButton() { }

#endif

    /// <summary>OnParticleCollision is called when a particle hits a collider.</summary>
    protected virtual void OnParticleCollision(GameObject other) { }

    /// <summary>OnPostRender is called after a camera finished rendering the scene.</summary>
    protected virtual void OnPostRender() { }

    /// <summary>OnPreCull is called before a camera culls the scene.</summary>
    protected virtual void OnPreCull() { }

    /// <summary>OnPreRender is called before a camera starts rendering the scene.</summary>
    protected virtual void OnPreRender() { }

    /// <summary>OnRenderImage is called after all rendering is complete to render image.</summary>
    protected virtual void OnRenderImage(RenderTexture src, RenderTexture dest) { }

    /// <summary>OnRenderObject is called after camera has rendered the scene.</summary>
    protected virtual void OnRenderObject() { }

    /// <summary>Called on the server whenever a Network.</summary>InitializeServer was invoked and has completed.</summary>
    protected virtual void OnServerInitialized() { }

    /// <summary>OnTriggerEnter is called when the Collider other enters the trigger.</summary>
    protected virtual void OnTriggerEnter(Collider other) { }

    /// <summary>Sent when another object enters a trigger collider attached to this object (2D physics only).</summary>
    protected virtual void OnTriggerEnter2D(Collider2D other) { }

    /// <summary>OnTriggerExit is called when the Collider other has stopped touching the trigger.</summary>
    protected virtual void OnTriggerExit(Collider other) { }

    /// <summary>Sent when another object leaves a trigger collider attached to this object (2D physics only).</summary>
    protected virtual void OnTriggerExit2D(Collider2D other) { }

    /// <summary>OnTriggerStay is called once per frame for every Collider other that is touching the trigger.</summary>
    protected virtual void OnTriggerStay(Collider other) { }

    /// <summary>Sent each frame where another object is within a trigger collider attached to this object (2D physics only).</summary>
    protected virtual void OnTriggerStay2D(Collider2D other) { }

    /// <summary>This function is called when the script is loaded or a value is changed in the inspector (Called in the editor only).</summary>
    protected virtual void OnValidate() { }

    /// <summary>OnWillRenderObject is called once for each camera if the object is visible.</summary>
    protected virtual void OnWillRenderObject() { }

    /// <summary>Reset to default values.</summary>
    protected virtual void Reset() { }

    /// <summary>Start is called on the frame when a script is enabled just before any of the Update methods is called the first time.</summary>
    protected virtual void Start() { }

    /// <summary>Update is called every frame, if the MonoBehaviour is enabled.</summary>
    protected virtual void Update() { }

#if !(UNITY_METRO || UNITY_WP8 || UNITY_NACL_CHROME || UNITY_WEBGL)
    /// <summary>Called on the client when the connection was lost or you disconnected from the server.</summary>
    protected virtual void OnDisconnectedFromServer(NetworkDisconnection info) { }

    /// <summary>Called on the client when a connection attempt fails for some reason.</summary>
    protected virtual void OnFailedToConnect(NetworkConnectionError error) { }

    /// <summary>Called on clients or servers when there is a problem connecting to the MasterServer.</summary>
    protected virtual void OnFailedToConnectToMasterServer(NetworkConnectionError info) { }

    /// <summary>Called on clients or servers when reporting events from the MasterServer.</summary>
    protected virtual void OnMasterServerEvent(MasterServerEvent msEvent) { }

    /// <summary>Called on objects which have been network instantiated with Network.</summary>Instantiate.</summary>
    protected virtual void OnNetworkInstantiate(NetworkMessageInfo info) { }

    /// <summary>Called on the server whenever a new player has successfully connected.</summary>
    protected virtual void OnPlayerConnected(NetworkPlayer player) { }

    /// <summary>Called on the server whenever a player disconnected from the server.</summary>
    protected virtual void OnPlayerDisconnected(NetworkPlayer player) { }

    /// <summary>Used to customize synchronization of variables in a script watched by a network view.</summary>
    protected virtual void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) { }
#endif
}
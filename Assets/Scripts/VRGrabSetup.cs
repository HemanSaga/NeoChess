using UnityEngine;


public class VRGrabSetup : MonoBehaviour
{
    void Start()
    {
        SetupAllPieces();
    }

    void SetupAllPieces()
    {
        // Find every chess piece in the scene
        ChessPiece[] allPieces = FindObjectsByType<ChessPiece>(FindObjectsSortMode.None);

        foreach (ChessPiece piece in allPieces)
        {
            SetupSinglePiece(piece.gameObject);
        }

        Debug.Log($"VR Grab setup complete for {allPieces.Length} pieces.");
    }

    void SetupSinglePiece(GameObject pieceRoot)
    {
        // 1. Add a Rigidbody so physics works
        Rigidbody rb = pieceRoot.GetComponent<Rigidbody>();
        if (rb == null)
            rb = pieceRoot.AddComponent<Rigidbody>();

        rb.useGravity  = true;
        rb.isKinematic = false;
        rb.mass        = 0.1f;   // light piece
        rb.linearDamping       = 5f;   // slows down quickly when released
        rb.angularDamping      = 5f;

        // 2. Make sure there's a collider on the root for grabbing
        //    Child primitives already have colliders, but we need one on root too
        Collider col = pieceRoot.GetComponent<Collider>();
        if (col == null)
        {
            // Add a capsule collider sized to wrap the piece
            CapsuleCollider cap = pieceRoot.AddComponent<CapsuleCollider>();
            cap.center = new Vector3(0, 0.04f, 0);
            cap.radius = 0.02f;
            cap.height = 0.1f;
        }

        // 3. Add XR Grab Interactable — this is what makes it grabbable in VR
        UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab = pieceRoot.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grab == null)
            grab = pieceRoot.AddComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

        // Keep the piece's world position when grabbed (no snapping to hand center)
        grab.attachTransform = null;
        grab.trackPosition   = true;
        grab.trackRotation   = true;
        grab.throwOnDetach   = false; // pieces shouldn't fly when released

        // 4. Set the interaction layer so only hands can grab pieces
        pieceRoot.layer = LayerMask.NameToLayer("Default");

        // 5. Add snap back behaviour
      if (pieceRoot.GetComponent<PieceSnapBack>() == null)
       pieceRoot.AddComponent<PieceSnapBack>();
    }
}
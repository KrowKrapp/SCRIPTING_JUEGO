using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayerControllerTests
{
    private GameObject playerObject;
    private PlayerControllerScript player;
    private Rigidbody2D rb;

    [SetUp]
    public void Setup()
    {
        // 1. Create the player object
        playerObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Object.DestroyImmediate(playerObject.GetComponent<BoxCollider>()); // Remove the 3D collider
        playerObject.AddComponent<BoxCollider2D>();

        // 2. Add Rigidbody2D first (PlayerControllerScript might need it in Awake)
        rb = playerObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 1f; // Ensure gravity is enabled for jump tests
        
        // 3. Add the PlayerControllerScript
        player = playerObject.AddComponent<PlayerControllerScript>();

        // 4. Set up groundCheck
        GameObject groundCheckObj = new GameObject("GroundCheck");
        groundCheckObj.transform.parent = playerObject.transform;
        // Adjust local position based on your player's pivot and size
        // For a 1x1 player, -0.5f is exactly at the bottom.
        // If your player has a collider, adjust this so groundCheck is slightly below it.
        groundCheckObj.transform.localPosition = new Vector3(0f, -0.5f, 0f);
        player.groundCheck = groundCheckObj.transform;

        // 5. Set public properties
        player.moveSpeed = 5f;
        player.jumpForce = 10f;
        player.groundCheckRadius = 0.2f; // Make sure this is set!

        // Ensure "Ground" layer exists for tests. This is for the test environment.
        // In a real project, you'd set this in Project Settings.
        // For testing, we can simulate its existence or ensure it's assigned.
        // Option A: If "Ground" layer exists, use GetMask.
        // Option B: Manually set a layer number if you want to control it entirely in tests.
        // For now, let's assume it's set up in Project Settings.
        player.groundLayer = LayerMask.GetMask("Ground");
        if (player.groundLayer == 0)
        {
            Debug.LogWarning("Ground layer not found. Ensure 'Ground' layer is set up in Project Settings -> Tags and Layers.");
            // Potentially add a layer for testing if it's missing.
            // This is more advanced and usually done once in the editor.
        }
        GameObject cameraObj = new GameObject("TestCamera");
        Camera cam = cameraObj.AddComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 5f;
        cam.transform.position = new Vector3(0, 0, -10); // Position it to see your test objects
                                                         // Optional: Ensure it's the main camera for rendering
        if (Camera.main == null)
        {
            cam.tag = "MainCamera";
        }
    }

    [TearDown]
    public void TearDown()
    {
        Object.Destroy(playerObject);
        GameObject cameraObj = GameObject.Find("TestCamera");
        if (cameraObj != null)
        {
            Object.Destroy(cameraObj);
        }
    }

    [UnityTest]
    public IEnumerator HandleMovement_MovesRight()
    {
        // Ensure initial velocity is zero before movement
        rb.linearVelocity = Vector2.zero; // <-- Sets to (0,0)
        yield return null; // Wait one frame for any initial Unity processing

        player.HandleMovement(1f); // <-- Calls the method that should set x velocity to moveSpeed
        yield return new WaitForFixedUpdate(); // <-- Waits for physics step to apply/confirm velocity

        Assert.Greater(player.GetVelocity().x, 0f, "El jugador debería moverse a la derecha."); // <-- Fails here, saying it's 0
        Assert.LessOrEqual(player.GetVelocity().x, player.moveSpeed, "La velocidad a la derecha no debe exceder moveSpeed.");
    }

    [UnityTest]
    public IEnumerator HandleMovement_MovesLeft()
    {
        rb.linearVelocity = Vector2.zero;
        yield return null;

        player.HandleMovement(-1f);
        yield return new WaitForFixedUpdate();

        Assert.Less(player.GetVelocity().x, 0f, "El jugador debería moverse a la izquierda.");
        Assert.GreaterOrEqual(player.GetVelocity().x, -player.moveSpeed, "La velocidad a la izquierda no debe exceder -moveSpeed.");
    }

    [UnityTest]
    public IEnumerator Jump_OnlyWhenGrounded()
    {
        // 1. Temporarily disable gravity for this specific test
        rb.gravityScale = 0f;

        // 2. Reset initial velocity
        rb.linearVelocity = Vector2.zero;

        // 3. Yield once to ensure Rigidbody state is processed before we start the jump logic
        yield return null;

        // 4. Manually set grounded state and immediately call Jump()
        // DO NOT yield between these two lines.
        player.SetGrounded(true);
        player.Jump();

        // 5. Wait for the physics update where the jump force would be applied
        yield return new WaitForFixedUpdate();

        // 6. Assertions
        Assert.Greater(player.GetVelocity().y, 0f, "Player should have jumped with a positive vertical velocity (gravity disabled).");
        // With gravity disabled, the velocity should be exactly jumpForce
        Assert.AreEqual(player.jumpForce, player.GetVelocity().y, 0.01f, "Jump force applied should be exactly jumpForce (within tolerance).");

        // 7. Restore gravity scale for good measure (though TearDown will destroy the object)
        rb.gravityScale = 1f;
    }

    [UnityTest]
    public IEnumerator Jump_DoesNotWorkWhenNotGrounded()
    {
        player.SetGrounded(false); // Manually set ungrounded
        rb.linearVelocity = Vector2.zero; // Reset velocity
        float initialY = rb.linearVelocity.y; // Should be 0

        yield return null; // Wait a frame

        player.Jump();
        yield return new WaitForFixedUpdate(); // Wait for physics update

        // The velocity should remain the same or decrease due to gravity, but not increase due to jump
        Assert.LessOrEqual(player.GetVelocity().y, initialY, "El jugador no debería saltar si no está en el suelo.");
    }

    [UnityTest]
    public IEnumerator GroundCheck_SetsIsGrounded()
    {
        // 1. Create a ground object
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Cube); // This creates a 3D MeshCollider by default
        ground.name = "Ground";
        ground.transform.localScale = new Vector3(10f, 1f, 1f);
        ground.transform.position = new Vector3(0f, -0.1f, 0f); // Position it below the player's groundCheck

        // 2. IMPORTANT: Remove the 3D MeshCollider and add a 2D Collider.
        // If you don't remove the MeshCollider, it might still interfere or be present.
        // However, adding a 2D collider is the critical part for 2D physics.
        Object.DestroyImmediate(ground.GetComponent<BoxCollider>()); // Remove the 3D collider
        ground.AddComponent<BoxCollider2D>(); // <-- THIS IS THE CRUCIAL LINE FOR 2D!

        Rigidbody2D groundRb = ground.AddComponent<Rigidbody2D>();
        groundRb.bodyType = RigidbodyType2D.Static; // Make it static so it doesn't move

        // 3. Set its layer to "Ground"
        int groundLayerIndex = LayerMask.NameToLayer("Ground");
        if (groundLayerIndex == -1)
        {
            Assert.Fail("The 'Ground' layer is not defined in Project Settings -> Tags and Layers. Please add it.");
        }
        else
        {
            ground.layer = groundLayerIndex;
        }

        playerObject.transform.position = Vector3.zero; // Ensure player starts at origin for predictable placement

        // 4. Ensure player's groundLayer is set to detect the 'Ground' layer
        // This is done in Setup, but good to re-emphasize it.
        // player.groundLayer = LayerMask.GetMask("Ground");

        // 5. Allow physics to settle. The player might fall onto the ground.
        // Call FixedUpdate multiple times to ensure the ground check logic runs reliably.
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate(); // Give it a few cycles to settle and detect

        // 6. Manually call FixedUpdate on the player to ensure the ground check logic is processed.
        player.FixedUpdate();

        Assert.IsTrue(player.GetGrounded(), "Player should be grounded after falling onto the ground.");

        Object.Destroy(ground); // Clean up the ground object
    }
}
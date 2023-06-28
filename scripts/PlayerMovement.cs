using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;


public class PlayerMovement : MonoBehaviour
{

    Vector3 movementInput = Vector3.zero;
    float movementSpeed = 0.07f;
    Vector3 rotationInput = Vector3.zero;
    float rotationSpeed = 1f;
    Vector3 headRotationInput = Vector3.zero;
    public float jumpStrenght = 5f;


    public int maxHealth = 100;
    public int currentHealth;
    float timerVal = 0;
    public float sprintModifier = 0.1f;
    private bool isGrounded = false;
    bool mouseclick = false;
    public int CurrentScene;

    //Item collection
    public bool WearingHelmet = false;
    public bool HoldingGun = false;
    public bool Ready = false;
    public bool run = false;

    public GameObject playerCamera;
    public Transform head;
    public TextMeshProUGUI HealthDisplay;
    public GameObject DeathMenu;
    public AudioSource walkingSound;


    public HealthBar healthBar;
    private void OnCollisionStay(Collision collision)

    {
        isGrounded = true;
        //Debug.Log("Im grounded");// to test if player is grounded
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Monkey")// Allows player to be damaged once Enemy approaches player
        {
            currentHealth -= 3;
            Debug.Log("player health:" + currentHealth);
            HealthDisplay.text = "Health:" + currentHealth;
            healthBar.SetHealth(currentHealth);
        }

        if (collision.gameObject.tag == "Teleporter 1" && Ready == true )
        {
            SceneManager.LoadScene(3);
        }

        if (collision.gameObject.tag == "Teleporter 1" && Ready == false)
        {
            
        }

        if (collision.gameObject.tag == "projectiles")
        {
           
            Debug.Log("Ouch");
            currentHealth -= 2;
            HealthDisplay.text = "Health:" + currentHealth;
            collision.gameObject.GetComponent<objectScript>().DestroyProjectiles();
            healthBar.SetHealth(currentHealth);
        }
    }

    void OnFire()
    {
        mouseclick = true;
    }
    void OnLook(InputValue value)
    {
        rotationInput.y = value.Get<Vector2>().x;
        headRotationInput.x = -value.Get<Vector2>().y;
    }
    // Start is called before the first frame update
    void OnMove(InputValue value)
    {
        movementInput = value.Get<Vector2>();
    }
    void OnJump()  //space to jump
    {
        if (isGrounded == true)
        {
            GetComponent<Rigidbody>().AddForce
            (Vector3.up * jumpStrenght, ForceMode.Impulse); //Lets player jump
        }
    }

    void OnSprint()
    {
        run = true;
    }

    private void Awake()
    {

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        DeathMenu.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        HealthDisplay.text = "Health: " + currentHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHealth > 0)
        {

            if (run == true)
            {
                movementSpeed = sprintModifier;
            }

            run = false;

            Debug.DrawLine(head.transform.position, head.transform.position + (head.transform.forward * 5f));
            RaycastHit hitInfo;
            if (Physics.Raycast(head.transform.position,
                head.transform.forward, out hitInfo, 15f))
            {
                // Player hits enemies with raycast and enemies take damage
                if (hitInfo.transform.tag == "Ranger" && mouseclick)
                {
                    Debug.Log("raycast hit: " + hitInfo.transform.gameObject.name);
                    hitInfo.transform.GetComponent<EnemyAI>().Hurt(); // Gets the enemyscript, and calls the functions with reduced the health of enemies
                    GetComponent<AudioSource>().Play(); //plays gun sounds when player clicks on enemy
                }

                if (hitInfo.transform.tag == "Monkey" && mouseclick)
                {
                    Debug.Log("raycast hit: " + hitInfo.transform.gameObject.name);
                    hitInfo.transform.GetComponent<EnemyMonkey>().Hurt(); // Gets the enemyscript, and calls the functions with reduced the health of enemies
                    GetComponent<AudioSource>().Play();
                }

                if (hitInfo.transform.tag == "helmet" && mouseclick)
                {
                    Debug.Log("raycast hit: " + hitInfo.transform.gameObject.name);
                    hitInfo.transform.GetComponent<objectScript>().Collected(); // Gets the enemyscript, and calls the functions with reduced the health of enemies
                    WearingHelmet = true;
                }

                if (hitInfo.transform.tag == "gun" && mouseclick)
                {
                    Debug.Log("raycast hit: " + hitInfo.transform.gameObject.name);
                    hitInfo.transform.GetComponent<objectScript>().Collected(); // Gets the enemyscript, and calls the functions with reduced the health of enemies
                    HoldingGun = true;
                }
            }

            if (HoldingGun == true && WearingHelmet == true)
            {
                Ready = true;
            }

            mouseclick = false; // detect if player clicked

            
            // Player movement 
            Vector3 forwardDir = transform.forward;
            forwardDir *= movementInput.y;

            Vector3 rightDir = transform.right;
            rightDir *= movementInput.x;

            GetComponent<Rigidbody>().MovePosition(transform.position + (forwardDir + rightDir) * movementSpeed);
            //transform.position += (forwardDir + rightDir) * movementSpeed;
            //if (gameObject.transform)
            //{
            //    if (Time.time > 3f)
            //    {
            //        walkingSound.Play();
            //        Debug.Log("Im walking");
            //    }
                
            //}
            // FOr player to look left and right
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + rotationInput * rotationSpeed);


            // For Player to look up and down
            var headRot = playerCamera.transform.rotation.eulerAngles
                + headRotationInput * rotationSpeed;

            //limitations for the player camera
            headRotationInput.x -= rotationInput.y;
            headRotationInput.x = Mathf.Clamp(headRotationInput.x, -45f, 45f);


            playerCamera.transform.rotation = Quaternion.Euler(headRot);

            isGrounded = false;

        }
        else // When Player dies
        {
            CurrentScene = SceneManager.GetActiveScene().buildIndex; // When player dies, this would track the scene int the player has died in
            DeathMenu.gameObject.SetActive(true);
            currentHealth = 50;
        }
    }
}

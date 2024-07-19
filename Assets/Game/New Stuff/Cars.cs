using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

namespace PoliceChase
{
    /// <summary>
    /// This script defines a car, which has health, speed, rotation speed, damage, and other attributes related to the car's behaviour. It also defines AI controls when the car is not player-controlled.
    /// </summary>
    public class Car : MonoBehaviour
    {
        // Various varialbes for quicker access
        internal Transform thisTransform;
        static GameController gameController;
        internal Transform chaseTarget;
        internal Vector3 targetPosition;
        internal RaycastHit groundHitInfo;
        internal Vector3 groundPoint;
        internal Vector3 forwardPoint;
        internal float forwardAngle;
        internal float rightAngle;

        [Tooltip("Il nome della macchina")]
        public string nomecar;

        public float health = 10;
        internal float healthMax;

        internal Transform healthBar;
        internal Image healthBarFill;

        public float hurtDelay = 3;
        internal float hurtDelayCount = 0;

        public Color hurtFlashColor = new Color(0.5f, 0.5f, 0.5f, 1);

        public float speed = 10;
        internal float speedMultiplier = 1;

        public float rotateSpeed = 200;
        internal float currentRotation = 0;

        public int damage = 1;

        public Transform hitEffect;

        public Transform deathEffect;

        public float driftAngle = 50;

        public float leanAngle = 10;

        public float leanReturnSpeed = 5;

        public Transform chassis;

        public Transform[] wheels;

        public int frontWheels = 2;

        internal int index;


        public float speedVariation = 2;

        // The angle range that AI cars try to chase the player at. So for example if 0 they will target the player exactly, while at 30 angle they stop rotating when they are at a 30 angle relative to the player
        internal float chaseAngle;

        public Vector2 chaseAngleRange = new Vector2(0, 30);

        public bool avoidObstacles = true;

        public float detectAngle = 2;

        public float detectDistance = 3;

        //internal float obstacleDetected = 0;

        public bool chaseOtherCars = true;
        
        public float changeTargetChance = 0.01f;
        private GameObject carFlashObject;
        private GameObject policeFlash;
        private GameObject macchinaBlu;
        private GameObject macchinaBianca;
        private GameObject pickup;
        private GameObject camioncino;
        private int carType;
        private ObjectManager objectManager;
        public AudioClip explodeSound;
        public AudioClip slamSound;
        private AudioSource audioSource;
        private bool isFlashing = false; 

        private void Start()
        {
            objectManager = FindObjectOfType<ObjectManager>();
            thisTransform = this.transform; 
            GameObject audioHolder = GameObject.Find("AudioHolder");
            audioSource = audioHolder.GetComponent<AudioSource>(); 
            if (gameObject.name == "Polizia(Clone)") {
                policeFlash = transform.Find("Police_CarFlash").gameObject;
                carType = -1;
            }
            if (gameObject.name == "CustomCar(Clone)")
            {
                //Imposta le variabili della macchina globali
                this.health = objectManager.getHP() / 10;
                this.damage = (int)objectManager.getDamage() / 10;
                this.speed = (objectManager.getSpeed() / 10) + 5;
                this.rotateSpeed = objectManager.getRotate() * 2;
                //Inizializzazione modelli
                carFlashObject = transform.Find("Vehicle_Car_Flash").gameObject;
                macchinaBianca = transform.Find("MacchinaBianca").gameObject;
                macchinaBlu = transform.Find("MacchinaBlu").gameObject;
                pickup = transform.Find("Pickup").gameObject;
                camioncino = transform.Find("Camioncino").gameObject;
                

                //Caso della macchina di Bill
                if (ObjectManager.instance.nomeMacchina == "Bill's Car")
                {
                    carType = 0;
                    macchinaBianca.GetComponent<MeshRenderer>().enabled = true;
                    macchinaBlu.GetComponent<MeshRenderer>().enabled = false;
                    pickup.GetComponent<MeshRenderer>().enabled = false;
                    camioncino.GetComponent<MeshRenderer>().enabled = false;
                }
                //Caso della macchina di Pablo
                if (ObjectManager.instance.nomeMacchina == "Pablo's Car")
                {
                    carType = 1;
                    macchinaBianca.GetComponent<MeshRenderer>().enabled = false;
                    macchinaBlu.GetComponent<MeshRenderer>().enabled = true;
                    pickup.GetComponent<MeshRenderer>().enabled = false;
                    camioncino.GetComponent<MeshRenderer>().enabled = false;
                }
                //Pikcup
                if (ObjectManager.instance.nomeMacchina == "Pickup")
                {
                    carType = 2;
                    macchinaBianca.GetComponent<MeshRenderer>().enabled = false;
                    macchinaBlu.GetComponent<MeshRenderer>().enabled = false;
                    pickup.GetComponent<MeshRenderer>().enabled = true;
                    camioncino.GetComponent<MeshRenderer>().enabled = false;
                }
                //Camioncino
                if (ObjectManager.instance.nomeMacchina == "Camioncino")
                {
                    carType = 3;
                    macchinaBianca.GetComponent<MeshRenderer>().enabled = false;
                    macchinaBlu.GetComponent<MeshRenderer>().enabled = false;
                    pickup.GetComponent<MeshRenderer>().enabled = false;
                    camioncino.GetComponent<MeshRenderer>().enabled = true;
                }
            }

            // Hold the gamecontroller for easier access
            if ( gameController == null )    gameController = GameObject.FindObjectOfType<GameController>();

            // If this AI car can chase other cars, choose one randomly from the scene
            if (chaseOtherCars == true )    ChooseTarget();
            else if (chaseTarget == null && gameController.gameStarted == true && gameController.playerObject) chaseTarget = gameController.playerObject.transform;
            
            RaycastHit hit;

            if (Physics.Raycast(thisTransform.position + Vector3.up * 5 + thisTransform.forward * 1.0f, -10 * Vector3.up, out hit, 100, gameController.groundLayer)) forwardPoint = hit.point;

            thisTransform.Find("Base").LookAt(forwardPoint);// + thisTransform.Find("Base").localPosition);

            // If this is not the player, then it is an AI controlled car, so we set some attribute variations for the AI such as speed and chase angle variations
            if (gameController.playerObject != this)
            {
                // Set a random chase angle for the AI car
                chaseAngle = Random.Range(chaseAngleRange.x, chaseAngleRange.y);

                // Set a random speed variation based on the original speed of the AI car
                speed += Random.Range(0, speedVariation);
            }

            // If there is a health bar in this car, assign it
            if ( thisTransform.Find("HealthBar") )
            {
                healthBar = thisTransform.Find("HealthBar");

                healthBarFill = thisTransform.Find("HealthBar/Empty/Full").GetComponent<Image>();
            }

            // Set the max health of the car
            healthMax = health;

            // Update the health value
            ChangeHealth(0);
        }

        public void ChooseTarget()
        {
            // Get all the cars in the scene
            Car[] allCars = GameObject.FindObjectsOfType<Car>();

            // Choose a random car
            int randomCar = Random.Range(0, allCars.Length);

            // Set the random car as the current chase target
            if ( allCars[randomCar] )    chaseTarget = allCars[randomCar].transform;
        }

        // This function runs whenever we change a value in the component
        private void OnValidate()
        {
            // Limit the maximum number of front wheels to the actual front wheels we have
            frontWheels = Mathf.Clamp(frontWheels, 0, wheels.Length);
        }

        // Update is called once per frame
        void Update()
        {
            // Directional controls and acceleration/stopping for player car
            if (gameController.playerObject == this)
            {
                /* (UNUSED CODE)
                // calculate the accelration of the car, as long as there is user input
                float acceleration = Mathf.Max(Mathf.Abs(Input.GetAxis("Horizontal")), Mathf.Abs(Input.GetAxis("Vertical")));

                // Smoothly change the speed multiplier to match the current accelration value
                speedMultiplier = Mathf.Lerp(speedMultiplier, acceleration, Time.deltaTime);
                */

                // Rotate the car until it reaches the desired chase angle from either side of the player
                if (Vector3.Angle(thisTransform.forward, targetPosition - thisTransform.position) > 0)
                {
                    Rotate(ChaseAngle(thisTransform.forward, targetPosition - thisTransform.position, Vector3.up));
                }

            }

            // If the game hasn't started yet, nothing happens
            if (gameController && gameController.gameStarted == false) return;

            // If we have no target, choose a random target to chase
            if ( chaseOtherCars == true && Random.value < changeTargetChance ) ChooseTarget();

            // Move the player forward
            thisTransform.Translate(Vector3.forward * Time.deltaTime * speed * speedMultiplier, Space.Self);

            // Get the current position of the target player
            if ( health > 0 )
            {
                if (chaseTarget) targetPosition = chaseTarget.transform.position;

                if (healthBar)    healthBar.LookAt(Camera.main.transform);
            }
            else
            {
                if (healthBar && healthBar.gameObject.activeSelf == true ) healthBar.gameObject.SetActive(false);
            }

            // Make the AI controlled car rotate towards the player
            if ( gameController.playerObject != this )
            {
                // Shoot a ray at the position to see if we hit something
                //Ray ray = new Ray(thisTransform.position + Vector3.up * 0.2f + thisTransform.right * Mathf.Sin(Time.time * 20) * detectAngle, transform.TransformDirection(Vector3.forward) * detectDistance);

                // Cast two raycasts to either side of the AI car so that we can detect obstacles
                Ray rayRight = new Ray(thisTransform.position + Vector3.up * 0.2f + thisTransform.right * detectAngle * 0.5f + transform.right * detectAngle * 0.0f * Mathf.Sin(Time.time * 50), transform.TransformDirection(Vector3.forward) * detectDistance);
                Ray rayLeft = new Ray(thisTransform.position + Vector3.up * 0.2f + thisTransform.right * -detectAngle * 0.5f - transform.right * detectAngle * 0.0f * Mathf.Sin(Time.time * 50), transform.TransformDirection(Vector3.forward) * detectDistance);

                RaycastHit hit;
                
                // If we detect an obstacle on our right side, swerve to the left
                if ( avoidObstacles == true && Physics.Raycast(rayRight, out hit, detectDistance) && (hit.transform.GetComponent<Ostacolo>() || (hit.transform.GetComponent<Car>() && gameController.playerObject != this)) )
                {
                    // Change the emission color of the obstacle to indicate that the car detected it
                    //if (hit.transform.GetComponent<MeshRenderer>() ) hit.transform.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", Color.red);

                    // Rotate left to avoid obstacle
                    Rotate(-1);

                    //obstacleDetected = 0.1f;
                }
                else if ( avoidObstacles == true && Physics.Raycast(rayLeft, out hit, detectDistance) && (hit.transform.GetComponent<Ostacolo>() || (hit.transform.GetComponent<Car>() && gameController.playerObject != this))) // Otherwise, if we detect an obstacle on our left side, swerve to the right
                {
                    // Change the emission color of the obstacle to indicate that the car detected it
                    //if (hit.transform.GetComponent<MeshRenderer>()) hit.transform.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", Color.red);

                    // Rotate right to avoid obstacle
                    Rotate(1);

                    //obstacleDetected = 0.1f;
                }
                else// if (obstacleDetected <= 0) // Otherwise, if no obstacle is detected, keep chasing the player normally
                {
                    // Rotate the car until it reaches the desired chase angle from either side of the player
                    if (Vector3.Angle(thisTransform.forward, targetPosition - thisTransform.position) > chaseAngle)
                    {
                        Rotate(ChaseAngle(thisTransform.forward, targetPosition - thisTransform.position, Vector3.up));
                    }
                    else // Otherwise, stop rotating
                    {
                        Rotate(0);
                    }
                }
            }

            // If we have no ground object assigned, or it is turned off, then cars will use raycast to move along terrain surfaces
            if ( gameController.groundObject == null || gameController.groundObject.gameObject.activeSelf == false )    DetectGround();

            //if (obstacleDetected > 0) obstacleDetected -= Time.deltaTime;

            // Count down the hurt delay, during which the car can't be hurt again
            if (hurtDelayCount > 0 && health > 0)
            {
                hurtDelayCount -= Time.deltaTime;
                if (!isFlashing)
                {
                    StartCoroutine(FlashRoutine());
                }
            }
            else
            {
                {
                    StopCoroutine(FlashRoutine());
                    isFlashing = false;
                    if (carType == 0 || carType == 1)
                    carFlashObject.GetComponent<MeshRenderer>().enabled = false;
                    if (carType == -1)
                    policeFlash.GetComponent<MeshRenderer>().enabled = false;
                }
            }
        }
        

        /// <summary>
        /// Calculates the approach angle of an object towrads another object
        /// </summary>
        /// <param name="forward"></param>
        /// <param name="targetDirection"></param>
        /// <param name="up"></param>
        /// <returns></returns>
        public float ChaseAngle(Vector3 forward, Vector3 targetDirection, Vector3 up)
        {
            // Calculate the approach angle
            float approachAngle = Vector3.Dot(Vector3.Cross(up, forward), targetDirection);
            
            // If the angle is higher than 0, we approach from the left ( so we must rotate right )
            if (approachAngle > 0f)
            {
                return 1f;
            }
            else if (approachAngle < 0f) //Otherwise, if the angle is lower than 0, we approach from the right ( so we must rotate left )
            {
                return -1f;
            }
            else // Otherwise, we are within the angle range so we don't need to rotate
            {
                return 0f;
            }
        }


        /// <summary>
        /// Rotates the car either left or right, and applies the relevant lean and drift effects
        /// </summary>
        /// <param name="rotateDirection"></param>
        public void Rotate( float rotateDirection )
        {
        if (chassis != null)
        {
            //thisTransform.localEulerAngles = new Vector3(Quaternion.FromToRotation(Vector3.up, groundHitInfo.normal).eulerAngles.x, thisTransform.localEulerAngles.y, Quaternion.FromToRotation(Vector3.up, groundHitInfo.normal).eulerAngles.z);

            //thisTransform.rotation = Quaternion.FromToRotation(Vector3.up, groundHitInfo.normal);


            // If the car is rotating either left or right, make it drift and lean in the direction its rotating
            if ( rotateDirection != 0 )
            {
                //thisTransform.localEulerAngles = Quaternion.FromToRotation(Vector3.up, groundHitInfo.normal).eulerAngles + Vector3.up * currentRotation;

                // Rotate the car based on the control direction
                thisTransform.localEulerAngles += Vector3.up * rotateDirection * rotateSpeed * Time.deltaTime;

                thisTransform.eulerAngles = new Vector3(thisTransform.eulerAngles.x, thisTransform.eulerAngles.y, thisTransform.eulerAngles.z);

                //thisTransform.eulerAngles = new Vector3(rightAngle, thisTransform.eulerAngles.y, forwardAngle);

                currentRotation += rotateDirection * rotateSpeed * Time.deltaTime;

                if (currentRotation > 360) currentRotation -= 360;
                //print(forwardAngle);
                // Make the base of the car drift based on the rotation angle
                thisTransform.Find("Base").localEulerAngles = new Vector3(rightAngle, Mathf.LerpAngle(thisTransform.Find("Base").localEulerAngles.y, rotateDirection * driftAngle + Mathf.Sin(Time.time * 50) * hurtDelayCount * 50, Time.deltaTime), 0);//  Mathf.LerpAngle(thisTransform.Find("Base").localEulerAngles.y, rotateDirection * driftAngle, Time.deltaTime);

                // Make the chassis lean to the sides based on the rotation angle
                if (chassis) chassis.localEulerAngles = Vector3.forward * Mathf.LerpAngle(chassis.localEulerAngles.z, rotateDirection * leanAngle, Time.deltaTime);//  Mathf.LerpAngle(thisTransform.Find("Base").localEulerAngles.y, rotateDirection * driftAngle, Time.deltaTime);
                
                // Play the skidding animation. In this animation you can trigger all kinds of effects such as dust, skid marks, etc
                GetComponent<Animator>().Play("Skid");

                // Go through all the wheels making them spin, and make the front wheels turn sideways based on rotation
                for (index = 0; index < wheels.Length; index++)
                {
                    // Turn the front wheels sideways based on rotation
                    if (index < frontWheels) wheels[index].localEulerAngles = Vector3.up * Mathf.LerpAngle(wheels[index].localEulerAngles.y, rotateDirection * driftAngle, Time.deltaTime * 10);
                }
            }
            else // Otherwise, if we are no longer rotating, straighten up the car and front wheels
            {
                // Return the base of the car to its 0 angle
                thisTransform.Find("Base").localEulerAngles = Vector3.up * Mathf.LerpAngle(thisTransform.Find("Base").localEulerAngles.y, 0, Time.deltaTime * 5);

                // Return the chassis to its 0 angle
                if (chassis) chassis.localEulerAngles = Vector3.forward * Mathf.LerpAngle(chassis.localEulerAngles.z, 0, Time.deltaTime * leanReturnSpeed);//  Mathf.LerpAngle(thisTransform.Find("Base").localEulerAngles.y, rotateDirection * driftAngle, Time.deltaTime);

                // Play the move animation. In this animation we stop any previously triggered effects such as dust, skid marks, etc
                GetComponent<Animator>().Play("Move");

                // Go through all the wheels making them spin faster than when turning, and return the front wheels to their 0 angle
                for (index = 0; index < wheels.Length; index++)
                {
                    // Return the front wheels to their 0 angle
                    if (index < frontWheels) wheels[index].localEulerAngles = Vector3.up * Mathf.LerpAngle(wheels[index].localEulerAngles.y, 0, Time.deltaTime * 5);
                }
            }
        }
        else
            {
            Debug.LogError("Chassis not assigned in Rotate function.");
            }
        }

        /// <summary>
        /// Is executed when this obstacle touches another object with a trigger collider
        /// </summary>
        /// <param name="other"><see cref="Collider"/></param>
        void OnTriggerStay(Collider other)
        {
            // If the hurt delay is over, and this car was hit by another car, damage it
            if ( hurtDelayCount <= 0  && other.GetComponent<Car>() )
            {
                // Reset the hurt delay
                hurtDelayCount = hurtDelay;

                // Damage the car
                audioSource.PlayOneShot(slamSound, 0.8f);
                other.GetComponent<Car>().ChangeHealth(-damage);

                // If there is a hit effect, create it
                if (health - damage > 0 && hitEffect) Instantiate(hitEffect, transform.position, transform.rotation);
            }
        }

        /// <summary>
        /// Changes the lives of the player. If lives reach 0, it's game over
        /// </summary>
        /// <param name="changeValue"></param>
        public void ChangeHealth(float changeValue)
        {
            // Change the health value
            health += changeValue;

            // Limit the value of the health to the maximum allowed value
            if (health > healthMax) health = healthMax;

            // Update the value in the health bar, if it exists
            if ( healthBar )
            {
                healthBarFill.fillAmount = health / healthMax;
            }

            // If this is the player car, play the shake animation
            if (changeValue < 0 && gameController.playerObject == this) Camera.main.transform.root.GetComponent<Animation>().Play();

            // If health reaches 0, the car dies
            if (health <= 0)
            {
                if (gameController.playerObject && gameController.playerObject != this)
                {
                    DelayedDie();
                }
                else
                {
                    Die();
                }

                if (gameController.playerObject && gameController.playerObject == this)
                {
                    gameController.SendMessage("GameOver", 1.2f);

                    // Carica la scena Game Over dopo un breve ritardo
                    Time.timeScale = 0.5f; //SLOWMOTION
                    gameController.dieEvents();
                }
            }

            if ( gameController.playerObject && gameController.playerObject == this && gameController.healthCanvas)
            {
                if (gameController.healthCanvas.Find("Full")) gameController.healthCanvas.Find("Full").GetComponent<Image>().fillAmount = health / healthMax;
                if (gameController.healthCanvas.Find("Text")) gameController.healthCanvas.Find("Text").GetComponent<Text>().text = health.ToString();
                if (gameController.healthCanvas.GetComponent<Animation>()) gameController.healthCanvas.GetComponent<Animation>().Play();
            }
        }

        /// <summary>
        /// Kill the car and create a death effect
        /// </summary>
        public void Die()
        {
            audioSource.PlayOneShot(explodeSound, 0.8f);  
            if (deathEffect) Instantiate(deathEffect, transform.position, transform.rotation);
            Destroy(gameObject);
        }

        /// <summary>
        /// Make the car lose control for a second, and then kill it
        /// </summary>
        public void DelayedDie()
        {
            //chaseTarget = null;

            // Add all wheels as part of the chasis to make sure they flip over with it
            for (index = 0; index < wheels.Length; index++)
            {
                wheels[index].transform.SetParent(chassis);
            }

            targetPosition = thisTransform.forward * -10;

            leanAngle = Random.Range(100,300);

            driftAngle = Random.Range(100, 150); ;

            //rotateSpeed *= 2;

            Invoke("Die", Random.Range(0,0.8f));
        }

        /// <summary>
        /// Detects the terrain under the car and aligns it to it
        /// </summary>
        public void DetectGround()
        {
            // Cast a ray to the ground below
            Ray carToGround = new Ray(thisTransform.position + Vector3.up * 10, -Vector3.up * 20);

            // Detect terrain under the car
            if (Physics.Raycast(carToGround, out groundHitInfo, 20, gameController.groundLayer))
            {
                //transform.position = new Vector3(transform.position.x, groundHitInfo.point.y, transform.position.z);
            }
            
            // Set the position of the car along the terrain
            thisTransform.position = new Vector3(thisTransform.position.x, groundHitInfo.point.y + 0.1f, thisTransform.position.z);

            RaycastHit hit;

            // Detect a point along the terrain in front of the car, so that we can make the car rotate accordingly
            if (Physics.Raycast(thisTransform.position + Vector3.up * 5 + thisTransform.forward * 1.0f, -10 * Vector3.up, out hit, 100, gameController.groundLayer))
            {
                forwardPoint = hit.point;
            }
            else if ( gameController.groundObject && gameController.groundObject.gameObject.activeSelf == true )
            {
                forwardPoint = new Vector3(thisTransform.position.x, gameController.groundObject.position.y, thisTransform.position.z);
            }

            // Make the car look at the point in front of it along the terrain
            thisTransform.Find("Base").LookAt(forwardPoint);
        }

        IEnumerator FlashRoutine()
        {
            isFlashing = true;
            while (hurtDelayCount > 0 && (carType == 0 || carType == 1)) //Se la macchina è di tipo 0 o di tipo 1 flasha con il mesh carflash
            {
                // Abilita la componente MeshRenderer
                carFlashObject.GetComponent<MeshRenderer>().enabled = true;

                // Attendere per 0.5 secondi
                yield return new WaitForSeconds(0.1f);

                // Disabilita la componente MeshRenderer
                carFlashObject.GetComponent<MeshRenderer>().enabled = false;

                // Attendere per 0.5 secondi prima del prossimo flash
                yield return new WaitForSeconds(0.1f);
            }

        //Caso in cui la macchina si tratta di una Police_Car (il mesh è diverso)
            while (hurtDelayCount > 0 && (carType == -1)) //Se la macchina è di tipo 0 o di tipo 1 flasha con il mesh carflash
            {
                // Abilita la componente MeshRenderer
                policeFlash.GetComponent<MeshRenderer>().enabled = true;

                // Attendere per 0.5 secondi
                yield return new WaitForSeconds(0.1f);

                // Disabilita la componente MeshRenderer
                policeFlash.GetComponent<MeshRenderer>().enabled = false;

                // Attendere per 0.5 secondi prima del prossimo flash
                yield return new WaitForSeconds(0.1f);
            }
            isFlashing = false;
        }

        public void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            Gizmos.DrawRay(transform.position + Vector3.up * 0.2f + transform.right * detectAngle * 0.5f + transform.right * detectAngle * 0.0f * Mathf.Sin(Time.time * 50), transform.TransformDirection(Vector3.forward) * detectDistance);
            Gizmos.DrawRay(transform.position + Vector3.up * 0.2f + transform.right * -detectAngle * 0.5f - transform.right * detectAngle * 0.0f * Mathf.Sin(Time.time * 50), transform.TransformDirection(Vector3.forward) * detectDistance);

            Gizmos.DrawSphere(forwardPoint, 0.5f);

            Gizmos.color = Color.yellow;

            Gizmos.DrawSphere(targetPosition, 0.3f);
        }
    }
}

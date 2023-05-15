using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [Header("Player settings")]
    public float moveSpeed;
    public float spawnTime;
    public float health = 100;

    public AudioClip[] gunShotSounds;

    [Header("Gun")]
    public int bullets;
    public float velocity, damage, cooldownTime, spread;
    public GameObject bullet;
    public ParticleSystem rightParticleSystem, leftParticleSystem;

    [Header("Dice settings")]
    public float rotationTime;
    public float leapTime;
    public Transform dice, levelsLeftIndicator;
    public Level.LevelsOnSide[] levels;

    public TMP_Text loseText;
    public GameObject finalLevel;

    private CharacterController charController;
    private Animator animator;
    private Transform sideHandler;
    private float currentAngle, targetAngle, angularVelocity;

    private bool inControl = false;

    private GameObject currentLevel;
    private float timeUntilFire;

    private bool rightSideNextFire = true;

    private int currentSide = 1;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        if (PlayerPrefs.GetInt("Lost") == 1)
        {
            PlayerPrefs.SetInt("Lost", 0);
            loseText.enabled = true;
        }

        charController = GetComponent<CharacterController>();
        animator = dice.GetChild(0).GetChild(0).GetComponent<Animator>();
        animator.speed = 1 / leapTime;
        sideHandler = dice.GetChild(1);
        StartCoroutine(ShowLevelsLeft(levels[currentSide - 1].levels.Count));

        float timeSpawned = 0;
        while (spawnTime > timeSpawned)
        {
            timeSpawned += Time.deltaTime;
            transform.position = Vector3.up * Mathf.SmoothStep(-5, 0.5f, timeSpawned / spawnTime);
            yield return null;
        }
        transform.position = Vector3.up * 0.5f;
        inControl = true;
        charController.enabled = true;

        yield return new WaitForSeconds(3f);

        loseText.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (inControl)
        {
            Vector3 movement = Vector3.down;
            movement.x = Input.GetAxis("Horizontal");
            movement.z = Input.GetAxis("Vertical");

            charController.Move(movement * moveSpeed * Time.deltaTime);

            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            float time = (0.5f - mouseRay.origin.y) / mouseRay.direction.y;
            Vector3 deltaMousePos = mouseRay.origin + mouseRay.direction * time - transform.position;
            targetAngle = Mathf.Atan(deltaMousePos.x / deltaMousePos.z) * Mathf.Rad2Deg + (deltaMousePos.z < 0 ? 180 : 0);
            currentAngle = Mathf.SmoothDampAngle(currentAngle, targetAngle, ref angularVelocity, 0.1f);
            transform.eulerAngles = Vector3.up * currentAngle;

            timeUntilFire -= Time.deltaTime;

            if (Input.GetKey(KeyCode.Mouse0) && timeUntilFire < 0)
            {
                timeUntilFire = cooldownTime;
                for (int i = 0; i < bullets; i++)
                {
                    GameObject temp = Instantiate(bullet, rightSideNextFire ? rightParticleSystem.transform.position : leftParticleSystem.transform.position, Quaternion.identity);
                    temp.transform.forward = transform.forward;
                    temp.transform.Rotate(Vector3.up * Random.Range(-spread, spread));
                    temp.GetComponent<Rigidbody>().velocity = velocity * temp.transform.forward;
                    temp.GetComponent<Bullet>().damage = damage;
                    Destroy(temp, 3);
                }
                GetComponent<AudioSource>().clip = gunShotSounds[Random.Range(0, gunShotSounds.Length)];
                GetComponent<AudioSource>().Play();
                if (rightSideNextFire)
                {
                    rightParticleSystem.Play();
                } else
                {
                    leftParticleSystem.Play();
                }
                rightSideNextFire = !rightSideNextFire;
            }
        }
    }

    void SpawnLevel()
    {
        StartCoroutine(ShowLevelsLeft(levels[currentSide - 1].levels.Count));
        if (levels[currentSide - 1].levels.Count > 0)
        {
            int index = Random.Range(0, levels[currentSide - 1].levels.Count);
            currentLevel = Instantiate(levels[currentSide - 1].levels[index], Vector3.zero, Quaternion.identity);
            levels[currentSide - 1].levels.RemoveAt(index);
            currentLevel.transform.LookAt(currentLevel.transform.position + sideHandler.GetChild(currentSide - 1).forward, sideHandler.GetChild(currentSide - 1).up);
            currentLevel.GetComponent<Level>().SetPlayer(this);
        } else
        {
            LevelSpawned();
        }
    }

    IEnumerator ShowLevelsLeft(int levelsLeft)
    {
        levelsLeftIndicator.parent = null;
        levelsLeftIndicator.eulerAngles = Vector3.zero;
        for (int i = 0; i < levelsLeftIndicator.childCount; i++)
        {
            Transform child = levelsLeftIndicator.GetChild(i);
            child.GetComponent<MeshRenderer>().enabled = i < levelsLeft;
            child.localPosition = Vector3.back * i;
        }
        float time = 0;
        while (time < spawnTime)
        {
            time += Time.deltaTime;
            levelsLeftIndicator.position = new Vector3(-10.3125f, Mathf.SmoothStep(-1.3125f, -0.3125f, time / spawnTime), 9.0625f);
            yield return null;
        }
        levelsLeftIndicator.position = new Vector3(-10.3125f, -0.3125f, 9.0625f);
    }

    IEnumerator HideLevelsLeft()
    {
        float time = 0;
        levelsLeftIndicator.parent = dice;
        Vector3 initialPosition = levelsLeftIndicator.localPosition;
        Vector3 direction = dice.InverseTransformDirection(Vector3.down);
        while (time < spawnTime)
        {
            time += Time.deltaTime;
            levelsLeftIndicator.localPosition = initialPosition + direction * Mathf.SmoothStep(0, 1, time / spawnTime);
            yield return null;
        }
        levelsLeftIndicator.localPosition = initialPosition + direction;
    }

    IEnumerator HideCurrentLevelIndicator()
    {
        Transform child = levelsLeftIndicator.GetChild(levels[currentSide - 1].levels.Count);
        float time = 0;
        while (time < spawnTime)
        {
            time += Time.deltaTime;
            child.localPosition = new Vector3(child.localPosition.x, Mathf.SmoothStep(0, -1, time / spawnTime), child.localPosition.z);
            yield return null;
        }
        child.localPosition = new Vector3(child.localPosition.x, -1, child.localPosition.z);
    }

    public void EnemyDestroyed()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length <= 1)
        {
            GameObject[] interactibles = GameObject.FindGameObjectsWithTag("Interactible");
            bool chestsOpened = true;
            for (int i  = 0; i < interactibles.Length; i++)
            {
                if (!interactibles[i].GetComponent<Animator>().enabled)
                {
                    chestsOpened = false;
                    break;
                }
            }
            if (chestsOpened)
            {
                StartCoroutine(HideCurrentLevelIndicator());
                bool won = true;
                for (int i = 0; i < levels.Length; i++)
                {
                    if (levels[i].levels.Count > 0)
                    {
                        won = false;
                        break;
                    }   
                }

                if (won)
                {
                    loseText.text = "Return to the 1 side";
                    levels[0].levels.Add(finalLevel);
                    StartCoroutine(HideLoseText(3));
                }
            }
        }
    }

    IEnumerator HideLoseText(float time)
    {
        loseText.enabled = true;
        yield return new WaitForSeconds(time);
        loseText.enabled = false;
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (inControl && hit.transform.tag == "Bound")
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            GameObject[] interactibles = GameObject.FindGameObjectsWithTag("Interactible");
            if (enemies.Length == 0)
            {
                bool canLeave = true;
                for (int i = 0; i < interactibles.Length; i++)
                {
                    if (!interactibles[i].GetComponent<Animator>().enabled)
                    {
                        canLeave = false;
                    }
                }
                if (canLeave)
                {
                    Vector3 boundPosition = hit.transform.position.normalized;
                    StartCoroutine(RotateDice(new Vector3(-boundPosition.z, 0, boundPosition.x)));
                }
            }
        }
    }

    IEnumerator RotateDice(Vector3 direction)
    {
        StartCoroutine(HideLevelsLeft());
        charController.enabled = false;
        inControl = false;
        GameObject[] textObjects = GameObject.FindGameObjectsWithTag("Text");

        if (currentLevel != null) {
            if (currentLevel.transform.childCount > 0) {
                float timeLowered = 0;
                while (timeLowered < spawnTime)
                {
                    timeLowered += Time.deltaTime;
                    currentLevel.transform.position = Vector3.up * Mathf.SmoothStep(0, -4.5f, timeLowered / spawnTime);
                    Vector3 textSize = new Vector3(1, 1 - Mathf.Clamp01 (timeLowered / (spawnTime * 0.33f)), 1);
                    for (int i = 0; i < textObjects.Length; i++)
                    {
                        textObjects[i].transform.localScale = textSize;
                    }
                    yield return null;
                }
            }
            Destroy(currentLevel);
        }

        animator.transform.localPosition = Vector3.zero;
        animator.transform.localEulerAngles = Vector3.zero;
        animator.transform.parent.position = transform.position;
        animator.transform.parent.LookAt(transform.position + new Vector3(direction.z, 0, -direction.x) * 100);
        transform.parent = animator.transform;
        animator.SetTrigger("Leap");

        float timeRotated = 0;
        Vector3 oldDiceRotation = dice.eulerAngles;
        while (timeRotated < rotationTime)
        {
            timeRotated += Time.deltaTime;
            
            if (transform.parent == animator.transform && timeRotated >= leapTime)
            {
                transform.parent = dice;
            }

            dice.eulerAngles = oldDiceRotation;
            dice.Rotate(direction * Mathf.SmoothStep(0, 90, Mathf.Clamp01(timeRotated / rotationTime)), Space.World);
            yield return null;
        }
        FindDiceSide();
        SpawnLevel();

        dice.eulerAngles = oldDiceRotation;
        dice.Rotate(direction * 90, Space.World);
        oldDiceRotation = dice.eulerAngles;
        oldDiceRotation.x = Mathf.Round(oldDiceRotation.x);
        oldDiceRotation.y = Mathf.Round(oldDiceRotation.y);
        oldDiceRotation.z = Mathf.Round(oldDiceRotation.z);
        dice.eulerAngles = oldDiceRotation;
        transform.parent = null;
    }

    public void LevelSpawned()
    {
        charController.enabled = true;
        inControl = true;
    }

    private int FindDiceSide()
    {
        float height = -20;
        for (int i = 0; i < sideHandler.childCount; i++)
        {
            if (sideHandler.GetChild(i).position.y > height)
            {
                height = sideHandler.GetChild(i).position.y;
                currentSide = i + 1;
            }
        }
        return currentSide;
    }

    public void Damage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            SceneManager.LoadScene(0);
            PlayerPrefs.SetInt("Lost", 1);
        }
    }
}

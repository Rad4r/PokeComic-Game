using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum BattleState {FIGHT, DEFEND, WON, DEFEATED}

public enum CombatState {FIGHT, DEFEND}

public class GameManager : MonoBehaviour
{
    [SerializeField] private ParticleSystem specialAttackEffect;
    [SerializeField] private int pokemonChoicePanel; //number 5 is the choice
    [SerializeField] private int pokemonBattlePanel; //number 13 is the choice
    [SerializeField] private int panelMoveSpeed;
    [SerializeField] private SpriteRenderer defeatedPanel;
    [SerializeField] private SpriteRenderer wonPanel;
    [SerializeField] private string pokemonChosen;
    [SerializeField] private TextMeshProUGUI battleStateText;

    [Header("Pokemon sprites")] 
    
    [SerializeField] private GameObject pokeChoiceCanvas;
    [SerializeField] private SpriteRenderer pokeballOutImage;
    [SerializeField] private Sprite[] pokemonOutSprites;
    [SerializeField] private GameObject pokeBattleCanvas;
    [SerializeField] private SpriteRenderer battlePokemon;
    [SerializeField] private Sprite[] battlePokemonSprites;
    
    [Header("BattlePokemon")]
    [SerializeField] private Entitiy myPokemon;
    [SerializeField] private Entitiy enemyPokemon;
    [SerializeField] private int myNormalAttackPower;
    [SerializeField] private int mySpecialAttackPower;
    [SerializeField] private int enemyAttackPower;

    [Header("PokemonAttacks")] 
    [SerializeField] private GameObject[] myAttacks;
    [SerializeField] private GameObject[] enemyAttacks;
    
    [SerializeField] private BattleState _battleState;
    [SerializeField] private CombatState _combatState;
    [SerializeField] private Transform lastPanel;
    private int _currentPokemonIndex; // 0 fire 1 grass 2 water
    private static ComicManager CM;
    public bool battleDone;
    
    //Combat objects
    private GameObject myPokemonAttack;
    private float timeBetweenAttacks = 3;
    private float attackTimer;
    private bool meAttacking;
    private int attackDirection; // 0 left 1 straight 2 right
    private Vector3 myAttackEndPosition = new Vector3(168.5f, 2.2f, -2f);
    private Vector3[] myAttackEndPositions;
    
    //Enemy combat
    private GameObject enemyAttackObj;
    private bool enemyAttacking;
    private int enemyAttackDirection;
    private Vector3 enemyAttackEndPosition = new Vector3(0f, -.7f, -2);
    private Vector3[] enemyAttackEndPositions;
    

    private bool myPokeAnimDone;
    private bool enemyAnimDone;

    private float battleTimer;

    void Start()
    {
        CM = FindObjectOfType<ComicManager>();
        myAttackEndPositions = new []{myAttackEndPosition + Vector3.left * 2f, myAttackEndPosition, myAttackEndPosition + Vector3.right * 1.5f};
        enemyAttackEndPositions = new []{enemyAttackEndPosition + Vector3.left * 2f, enemyAttackEndPosition, enemyAttackEndPosition + Vector3.right * 1.5f};
        //Check if it is the pokemon choice panel
        _battleState = BattleState.FIGHT; // change this somewhere else
        _combatState = CombatState.DEFEND;

    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log(battleTimer);
        if (battleDone)
        {
            if (_battleState == BattleState.WON)
                StartCoroutine(UpdateEndPanel(wonPanel));
                
            else if (_battleState == BattleState.DEFEATED)
                StartCoroutine(UpdateEndPanel(defeatedPanel));
        }
        
        if(meAttacking)
            MyAttack();
        
        if(enemyAttacking)
            EnemyAttack();
        
        if (CM.GetCurrentPanelNumber() == pokemonChoicePanel) //temporarily disable the continue for a few seconds
            PokemonChoice();
        else if (CM.GetCurrentPanelNumber() == pokemonBattlePanel && !battleDone)//check if it is the last panel to start the fight
            BattleScreenNew();
    }

    void PokemonChoice()
    {
        if (Vector3.Distance(Camera.main.transform.position,
            CM.GetCurrentPanelBoard().transform.position + new Vector3(0, 0, -10)) > 0)
            return;
        
        //pause for a bit and display options
        pokeChoiceCanvas.SetActive(true);
        
        if (Input.GetButtonDown("RedButton"))
        {
            pokemonChosen = "Charmander"; //move to next panel after choice made
            pokeballOutImage.sprite = pokemonOutSprites[0];
            battlePokemon.sprite = battlePokemonSprites[0];
            CM.ChangePanelNumber(pokemonChoicePanel + 1);
            ParticleSystem.MainModule settings = specialAttackEffect.main;
            settings.startColor = new ParticleSystem.MinMaxGradient( new Color32(255, 2, 0, 255) );
            _currentPokemonIndex = 0;
            pokeChoiceCanvas.SetActive(false);
        }
        else if (Input.GetButtonDown("GreenButton"))
        {
            pokemonChosen = "BulbaSquirtle";
            pokeballOutImage.sprite = pokemonOutSprites[1];
            battlePokemon.sprite = battlePokemonSprites[1];
            CM.ChangePanelNumber(pokemonChoicePanel + 1);
            ParticleSystem.MainModule settings = specialAttackEffect.main;
            settings.startColor = new ParticleSystem.MinMaxGradient( new Color(0, 153, 0));
            _currentPokemonIndex = 1;
            pokeChoiceCanvas.SetActive(false);
        }
        else if (Input.GetButtonDown("BlueButton"))
        {
            pokemonChosen = "Squirtasaur";
            pokeballOutImage.sprite = pokemonOutSprites[2];
            battlePokemon.sprite = battlePokemonSprites[2];
            CM.ChangePanelNumber(pokemonChoicePanel + 1);
            ParticleSystem.MainModule settings = specialAttackEffect.main;
            settings.startColor = new ParticleSystem.MinMaxGradient(new Color(0, 128, 255));
            _currentPokemonIndex = 2;
            pokeChoiceCanvas.SetActive(false);
        }
        
    }

    void BattleScreenNew()
    {
        if (Vector3.Distance(Camera.main.transform.position,
        CM.GetCurrentPanelBoard().transform.position + new Vector3(0, 0, -10)) > 0)
        return;
        
        if (myPokemon.GetDamaged(0) <= 0)
        {
            StopAllCoroutines();
            _battleState = BattleState.DEFEATED;
        }
            
        else if (enemyPokemon.GetDamaged(0) <= 0)
        {
            StopAllCoroutines();
            _battleState = BattleState.WON;
        }

        if (Input.GetButtonDown("SwitchAttack")) //CHANGE Based on what the input is (On or Off)
        {
            switch (_combatState)
            {
                case CombatState.DEFEND:
                    _combatState = CombatState.FIGHT;
                    battleStateText.text = "Combat State: Fighting";
                    break;
                case CombatState.FIGHT: 
                    _combatState = CombatState.DEFEND;
                    battleStateText.text = "Combat State: Dodging";
                    break;
            }
        }


        switch (_combatState)
        {
            case CombatState.DEFEND: //Check if the pokemon is hit here maybe and add colliders to the prefabs maybe
                myPokemon.transform.localPosition = new Vector3(Mathf.Clamp(myPokemon.transform.localPosition.x, -1.5f, 1.5f),Mathf.Clamp(myPokemon.transform.localPosition.y, -.7f, 0.35f), -2f);
                //cOULD BE BETTER BUT LEAVE IT
                if (Input.GetButtonDown("RedButton")) //move pokemon left
                {
                    myPokemon.transform.localPosition += Vector3.left * 2f; // not hard code this value
                    Invoke("PokemonPositionReset", .5f);
                    
                }
                else if (Input.GetButtonDown("GreenButton")) //move pokemon left
                {
                    myPokemon.transform.localPosition += Vector3.up * 1.5f; // not hard code this value
                    Invoke("PokemonPositionReset", .5f);
                    //move pokemon up
                }
                else if (Input.GetButtonDown("BlueButton")) //move pokemon left
                {
                    myPokemon.transform.localPosition += Vector3.right * 1.5f; // not hard code this value
                    Invoke("PokemonPositionReset", .5f);
                    //move pokemon right
                }
                break;
            case CombatState.FIGHT: //change the attack sprite according to current pokemon
                Vector3 myAttackStartPosition = new Vector3(167.56f, 0.85f, -1f);
                
                if (Input.GetButtonDown("RedButton"))
                {
                    myPokemonAttack = Instantiate(myAttacks[_currentPokemonIndex], myAttackStartPosition, Quaternion.identity, lastPanel);
                    myPokemonAttack.transform.localScale = Vector3.zero;
                    attackDirection = 0;
                    meAttacking = true;
                }
                else if (Input.GetButtonDown("GreenButton")) // doesn't update
                {
                    myPokemonAttack = Instantiate(myAttacks[_currentPokemonIndex], myAttackStartPosition, Quaternion.identity, lastPanel); 
                    myPokemonAttack.transform.localScale = Vector3.zero;
                    attackDirection = 1;
                    meAttacking = true;
                }
                else if (Input.GetButtonDown("BlueButton")) //Move towards might need to be in update or sperate and use bool to chec if button pressed
                {
                    myPokemonAttack = Instantiate(myAttacks[_currentPokemonIndex], myAttackStartPosition, Quaternion.identity, lastPanel); 
                    myPokemonAttack.transform.localScale = Vector3.zero;
                    attackDirection = 2;
                    meAttacking = true;
                    //Attack pokemon right
                }
                break;
        }
        
        switch (_battleState) //collision detection required
        {
            case BattleState.DEFEATED: pokeBattleCanvas.SetActive(false);
                battleDone = true;
                break;
            case BattleState.WON: pokeBattleCanvas.SetActive(false);
                battleDone = true;
                break;
            case BattleState.DEFEND: //delay and attack next
                battleTimer += Time.deltaTime;
                MoveBird();
                
                
                break;
            case BattleState.FIGHT: //delay and move next  attack from where it stopped
                battleTimer += Time.deltaTime;
                BirdAttack();
                break;
        }
        
        
    }

    void MyAttack() //end position needs to be adjusted to the right position
    {
        
        if (myPokemonAttack.transform.position == myAttackEndPositions[attackDirection])
            meAttacking = false;
        
        myPokemonAttack.transform.localScale = Vector3.MoveTowards(myPokemonAttack.transform.localScale, Vector3.one, Time.deltaTime * 10f);
        myPokemonAttack.transform.position = Vector3.MoveTowards(myPokemonAttack.transform.position, myAttackEndPositions[attackDirection],
            Time.deltaTime * 10f);
        Destroy(myPokemonAttack, 1f);
        
    }

    void BirdAttack() //make the attacks dissapper after a while
    {
        if (battleTimer > 10f)
        {
            _battleState = BattleState.DEFEND;
            battleTimer = 0;
        }

        if (battleTimer % 2 < 0.005 && !enemyAttacking)
        {
            Vector3 defaultPosition = new Vector3(0, 1f, -1);
            Vector3[] positions = new [] {defaultPosition + Vector3.left * 2f, defaultPosition, defaultPosition + Vector3.right * 1.5f};
            enemyAttackDirection = Random.Range(0, 3);
            
            Debug.Log("attackSpawned");
            //Instantiate not spawning??
            enemyAttackObj = Instantiate(enemyAttacks[enemyAttackDirection], positions[enemyAttackDirection], Quaternion.identity, lastPanel);
            enemyAttackObj.transform.localPosition = defaultPosition;
            enemyAttackObj.transform.localScale = new Vector3(.2f,.2f,.2f);
            enemyAttacking = true;
            
        }

    }
    void MoveBird() // update could launch the bird too far away
    {
        Vector3 defaultPosition = new Vector3(.8f, .8f, -1);
        
        if (battleTimer > 10f)
        {
            _battleState = BattleState.FIGHT;
            battleTimer = 0;
        }
        // Debug.Log("battleTimer % 2 = " + battleTimer % 2 );
        if (battleTimer % 2 < 0.005)
        {
            // Debug.Log("Bird move activated");
            int movePos = Random.Range(0, 3);
            Vector3[] positions = {defaultPosition + Vector3.left * 2f, defaultPosition, defaultPosition + Vector3.right * 1.5f};
            enemyPokemon.transform.localPosition = positions[movePos];
        }
        
    }

    void EnemyAttack()
    {
        if (enemyAttackObj.transform.localPosition == enemyAttackEndPositions[enemyAttackDirection])
            enemyAttacking = false;
       
        enemyAttackObj.transform.localScale = Vector3.MoveTowards(enemyAttackObj.transform.localScale, Vector3.one, Time.deltaTime * 10f);
        enemyAttackObj.transform.localPosition = Vector3.MoveTowards(enemyAttackObj.transform.localPosition, enemyAttackEndPositions[enemyAttackDirection],
            Time.deltaTime * 10f);
        Destroy(enemyAttackObj, 1f);
    }

    void EnemyPokemonRestPosition()
    {
        enemyPokemon.transform.localPosition = new Vector3(.8f, .8f, -1);
    }
    void PokemonPositionReset()
    {
        myPokemon.transform.localPosition = new Vector3(0, -0.7f, -2);
    }
    
    // void BattleScreen() //maybe I enumerator and update the images on the screen with info
    // {
    //     if (Vector3.Distance(Camera.main.transform.position,
    //         CM.GetCurrentPanelBoard().transform.position + new Vector3(0, 0, -10)) > 0)
    //         return;
    //
    //     // Debug.Log(_battleState + " now the battle is " + battleDone);
    //
    //     if (myPokemon.GetDamaged(0) <= 0)
    //     {
    //         StopAllCoroutines();
    //         _battleState = BattleState.DEFEATED;
    //     }
    //         
    //     else if (enemyPokemon.GetDamaged(0) <= 0)
    //     {
    //         StopAllCoroutines();
    //         _battleState = BattleState.WON;
    //     }
    //         
    //
    //     switch (_battleState)
    //     {
    //         case BattleState.DEFEATED: pokeBattleCanvas.SetActive(false);
    //              //cannot update becude battle done
    //             battleDone = true;
    //             break;
    //         case BattleState.WON: pokeBattleCanvas.SetActive(false);
    //             // Move to won panel
    //             battleDone = true;
    //             break;
    //         case BattleState.FIGHT: //CancelInvoke();
    //             pokeBattleCanvas.SetActive(true); //need animations and invoke (delay) the state range after animation
    //         
    //             if (Input.GetKeyDown(KeyCode.LeftArrow)) // norm attack
    //             {
    //                 myPokemon.GetComponent<Animator>().SetBool("Attack", true);
    //                 enemyPokemon.GetDamaged(myNormalAttackPower); //add animation here
    //                 StartCoroutine(DelayedStateChange(BattleState.DEFEND));
    //             }
    //             else if (Input.GetKeyDown(KeyCode.UpArrow)) //Leer increease attack by .2f
    //             {
    //                 myPokemon.GetComponent<Animator>().SetBool("Leer", true);
    //                 myNormalAttackPower += 2;
    //                 mySpecialAttackPower += 2;
    //                 StartCoroutine(DelayedStateChange(BattleState.DEFEND));
    //                 // Invoke("DelayedDefendState", 1f);
    //             }
    //             else if (Input.GetKeyDown(KeyCode.RightArrow)) //Adjust the text here and attack animation
    //             {
    //                 // myPokemon.GetComponent<Animator>().Play("pokemon-specialAttack");
    //                 myPokemon.GetComponent<Animator>().SetBool("SpecialAttack", true);
    //                 enemyPokemon.GetDamaged(mySpecialAttackPower); //add animation here
    //                 StartCoroutine(DelayedStateChange(BattleState.DEFEND));
    //                 // Invoke("DelayedDefendState", 1f);
    //             }
    //             break;
    //         case BattleState.DEFEND: pokeBattleCanvas.SetActive(false); //constant dameage here
    //             // enemyPokemon.GetComponent<Animator>().Play("enemy-attack");
    //             enemyPokemon.GetComponent<Animator>().SetBool("enemyAttack", true);
    //
    //             if (!enemyAnimDone)
    //             {
    //                 myPokemon.GetDamaged(enemyAttackPower); //add animation here
    //                 enemyAnimDone = true;
    //             }
    //             
    //             // Invoke("DelayedFightState", 1f);
    //             StartCoroutine(DelayedStateChange(BattleState.FIGHT));
    //             
    //
    //             // if (enemyPokemon.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("enemy-attack"))
    //             // {
    //             //     enemyPokemon.GetComponent<Animator>().SetBool("enemyAttack", false);
    //             //     _battleState = BattleState.FIGHT;
    //             //     enemyAnimDone = false;
    //             // }
    //             
    //             // _battleState = BattleState.FIGHT;// then back to Attack
    //             break;
    //         
    //             
    //     }

    // }

    // IEnumerator DelayedStateChange(BattleState state)
    // {
    //     yield return new WaitForSeconds(.5f);
    //     
    //     if (state == BattleState.DEFEND)
    //     {
    //         myPokemon.GetComponent<Animator>().SetBool("Leer", false);
    //         myPokemon.GetComponent<Animator>().SetBool("SpecialAttack", false);
    //         myPokemon.GetComponent<Animator>().SetBool("Attack", false);
    //         // pokeBattleCanvas.SetActive(true);
    //     }
    //     else
    //     {
    //         enemyPokemon.GetComponent<Animator>().SetBool("enemyAttack", false);
    //         enemyAnimDone = false;
    //     }
    //     
    //     _battleState = state;
    // }
    // private void DelayedFightState()
    // {
    //     // enemyPokemon.GetComponent<Animator>().Play("enemy-attack");
    //     enemyPokemon.GetComponent<Animator>().SetBool("enemyAttack", false);
    //     _battleState = BattleState.FIGHT;
    // }
    //
    // private void DelayedDefendState()
    // {
    //     // myPokemon.GetComponent<Animator>().Play("pokemon-normalAttack");
    //     myPokemon.GetComponent<Animator>().SetBool("Leer", false);
    //     myPokemon.GetComponent<Animator>().SetBool("SpecialAttack", false);
    //     myPokemon.GetComponent<Animator>().SetBool("Attack", false);
    //     _battleState = BattleState.DEFEND;
    // }

    IEnumerator UpdateEndPanel(SpriteRenderer panel)
    {
        yield return new WaitForSeconds(.5f);
        
        Camera.main.transform.position = Vector3.MoveTowards(Camera.main.transform.position,
            panel.transform.position + new Vector3(0, 0, -10), panelMoveSpeed * Time.deltaTime );
        Camera.main.orthographicSize = Mathf.MoveTowards(Camera.main.orthographicSize, panel.GetComponent<SpriteRenderer>().bounds.size.y / 1.7f, Time.deltaTime);
    }
}

using System.Collections;
using System.Collections.Generic;
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

    private bool myPokeAnimDone;
    private bool enemyAnimDone;

    private float battleTimer;

    void Start()
    {
        CM = FindObjectOfType<ComicManager>();
        //Check if it is the pokemon choice panel
        _battleState = BattleState.DEFEND; // change this somewhere else
        _combatState = CombatState.DEFEND;

    }

    // Update is called once per frame
    void Update()
    {
        if (battleDone)
        {
            // Debug.Log("battle is done");
            // if (_battleState == BattleState.WON)
            // {
            //     StartCoroutine(UpdateEndPanel(wonPanel));
            // }
            //     
            // else if (_battleState == BattleState.DEFEATED)
            // {
            //     StartCoroutine(UpdateEndPanel(defeatedPanel));
            // }
                
        }
        
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
                    break;
                case CombatState.FIGHT: 
                    _combatState = CombatState.DEFEND;
                    break;
            }
        }


        switch (_combatState)
        {
            case CombatState.DEFEND: //Check if the pokemon is hit here maybe and add colliders to the prefabs maybe
                //Or time it to check for damage
                if (Input.GetButtonDown("RedButton"))
                {
                    myPokemon.transform.position += Vector3.left * 3; // not hard code this value
                    Invoke("PokemonPositionReset", .5f);
                    //move pokemon left
                }
                else if (Input.GetButtonDown("GreenButton"))
                {
                    myPokemon.transform.position += Vector3.up * 3; // not hard code this value
                    Invoke("PokemonPositionReset", .5f);
                    //move pokemon up
                }
                else if (Input.GetButtonDown("BlueButton"))
                {
                    myPokemon.transform.position += Vector3.right * 3; // not hard code this value
                    Invoke("PokemonPositionReset", .5f);
                    //move pokemon right
                }
                break;
            case CombatState.FIGHT: //change the attack sprite according to current pokemon
                Vector3 startPosition = new Vector3(167.56f, 0.85f, -1f);
                
                if (Input.GetButtonDown("RedButton"))
                {
                    Vector3 endPosition = new Vector3(165.5f, 2.2f, -2f); //the enemy position
                    GameObject attackObj = Instantiate(myAttacks[_currentPokemonIndex], startPosition, Quaternion.identity, lastPanel); //position it to the left
                    attackObj.transform.localScale = Vector3.zero;
                    attackObj.transform.localScale = Vector3.MoveTowards(attackObj.transform.localScale, Vector3.one, Time.deltaTime * 10f);
                    attackObj.transform.position = Vector3.MoveTowards(attackObj.transform.position, endPosition,
                        Time.deltaTime * 10f);
                    //Also make it grow in scale
                    //Attack pokemon left
                }
                else if (Input.GetButtonDown("GreenButton"))
                {
                    Vector3 endPosition = new Vector3(168.5f, 2.2f, -2f);
                    GameObject attackObj = Instantiate(myAttacks[_currentPokemonIndex], startPosition, Quaternion.identity, lastPanel); 
                    attackObj.transform.localScale = Vector3.zero;
                    attackObj.transform.localScale = Vector3.MoveTowards(attackObj.transform.localScale, Vector3.one, Time.deltaTime * 10f);
                    attackObj.transform.position = Vector3.MoveTowards(attackObj.transform.position, endPosition,
                        Time.deltaTime * 10f);
                    //Attack pokemon up
                }
                else if (Input.GetButtonDown("BlueButton")) //Move towards might need to be in update or sperate and use bool to chec if button pressed
                {
                    Vector3 endPosition = new Vector3(170.5f, 2.2f, -2f);
                    GameObject attackObj = Instantiate(myAttacks[_currentPokemonIndex], startPosition, Quaternion.identity, lastPanel);
                    attackObj.transform.localScale = Vector3.zero;
                    attackObj.transform.localScale = Vector3.MoveTowards(attackObj.transform.localScale, Vector3.one, Time.deltaTime * 10f);
                    attackObj.transform.position = Vector3.MoveTowards(attackObj.transform.position, endPosition,
                        Time.deltaTime * 10f);
                    //Attack pokemon right
                }
                break;
        }
        
        switch (_battleState) //collision detection required
        {
            case BattleState.DEFEATED: pokeBattleCanvas.SetActive(false);
                 //cannot update becude battle done
                battleDone = true;
                break;
            case BattleState.WON: pokeBattleCanvas.SetActive(false);
                // Move to won panel
                battleDone = true;
                break;
            case BattleState.DEFEND: //delay and attack next
                //change to fight after a few seconds
                battleTimer += Time.deltaTime;
                StartCoroutine(MoveBird());
                
                //Bird attacks in different directions
                
                break;
            case BattleState.FIGHT: //delay and move next  attack from where it stopped
                Vector3 defaultPosition = new Vector3(.8f, .8f, -1);
                //Ienumerator and shoot from current bird post to pokemon
                StartCoroutine(BirdAttack());
                
                // enemyPokemon.transform.position = 
                //Bird moves around 
                break;
        }
        
        
    }

    IEnumerator BirdAttack() //make the attacks dissapper after a while
    {
        Vector3 defaultPosition = new Vector3(0, 1f, -1);
        // Vector3 defaultEndPosition = new Vector3(0f, -.7f, -2);
        Vector3[] positions = {defaultPosition + Vector3.left, defaultPosition, defaultPosition + Vector3.right};
        // Vector3[] endPositions = {defaultEndPosition + Vector3.left, defaultEndPosition, defaultEndPosition + Vector3.right};
        int attackPos = Random.Range(0, 3);

        GameObject enemyAttackObj = Instantiate(enemyAttacks[attackPos], positions[attackPos], Quaternion.identity, lastPanel);
        enemyAttackObj.transform.localScale = new Vector3(.2f,.2f,.2f);
        enemyAttackObj.transform.localScale = Vector3.MoveTowards(enemyAttackObj.transform.localScale, Vector3.one, Time.deltaTime * 10f);
        // enemyAttackObj.transform.position = Vector3.MoveTowards(enemyAttackObj.transform.position, endPositions[attackPos],
        //     Time.deltaTime * 10f);
        
        yield return new WaitForSeconds(.5f);
        
        if (battleTimer < 10f)
            StartCoroutine(MoveBird());
        else
        {
            _battleState = BattleState.DEFEND;
            battleTimer = 0;
        }
            
    }
    IEnumerator MoveBird() // update could launch the bird too far away
    {
        Vector3 defaultPosition = new Vector3(.8f, .8f, -1);
        int movePos = Random.Range(0, 3);
        Vector3[] positions = {defaultPosition + Vector3.left * 3f, defaultPosition, defaultPosition + Vector3.right * 3f};

        enemyPokemon.transform.position = positions[movePos];
        //might have to use local positions
        yield return new WaitForSeconds(.5f);

        if (battleTimer < 10f)
            StartCoroutine(MoveBird());
        else
        {
            _battleState = BattleState.FIGHT;
            battleTimer = 0;
        }
            
        // if (_battleState == BattleState.DEFEND)
        //     StartCoroutine(MoveBird());
    }

    void EnemyPokemonRestPosition()
    {
        enemyPokemon.transform.position = new Vector3(.8f, .8f, -1);
    }
    void PokemonPositionReset()
    {
        myPokemon.transform.position = new Vector3(0, -0.7f, -2);
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

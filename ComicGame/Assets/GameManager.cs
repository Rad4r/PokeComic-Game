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
    
    // [SerializeField] private BattleState _battleState;
    [SerializeField] private CombatState _combatState;
    private static ComicManager CM;
    public bool battleDone;

    private bool myPokeAnimDone;
    private bool enemyAnimDone;

    void Start()
    {
        CM = FindObjectOfType<ComicManager>();
        //Check if it is the pokemon choice panel
        // _battleState = BattleState.FIGHT; // change this somewhere else
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
            pokeChoiceCanvas.SetActive(false);
        }
        else if (Input.GetButtonDown("GreenButton"))
        {
            pokemonChosen = "Squirtle";
            pokeballOutImage.sprite = pokemonOutSprites[1];
            battlePokemon.sprite = battlePokemonSprites[1];
            CM.ChangePanelNumber(pokemonChoicePanel + 1);
            ParticleSystem.MainModule settings = specialAttackEffect.main;
            settings.startColor = new ParticleSystem.MinMaxGradient( new Color(0, 128, 255) );
            pokeChoiceCanvas.SetActive(false);
        }
        else if (Input.GetButtonDown("BlueButton"))
        {
            pokemonChosen = "Bulbasaur";
            pokeballOutImage.sprite = pokemonOutSprites[2];
            battlePokemon.sprite = battlePokemonSprites[2];
            CM.ChangePanelNumber(pokemonChoicePanel + 1);
            ParticleSystem.MainModule settings = specialAttackEffect.main;
            settings.startColor = new ParticleSystem.MinMaxGradient( new Color(0, 153, 0) );
            pokeChoiceCanvas.SetActive(false);
        }
        
    }

    void BattleScreenNew()
    {
        if (Vector3.Distance(Camera.main.transform.position,
        CM.GetCurrentPanelBoard().transform.position + new Vector3(0, 0, -10)) > 0)
        return;
        
        
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

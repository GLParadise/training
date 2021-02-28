package main

import(
	"strconv"
	"log"
	"os"
	"time"
	"fmt"
	"strings"
	"math/rand" //for computer choice
	"encoding/csv" //for reading csv files
)
var NUM_PKMN_TYPES int = 18
var pkmnList [18]string = [18]string{"normal","fire","water","electric","grass","ice","fighting","poison","ground","flying","psychic","bug","rock","ghost","dragon","dark","steel","fairy"}
var NORMAL_INDEX int = 0
var FIRE_INDEX int = 1
var WATER_INDEX int = 2
var ELECTRIC_INDEX int = 3
var GRASS_INDEX int = 4
var ICE_INDEX int = 5
var FIGHTING_INDEX int = 6
var POISON_INDEX int = 7
var GROUND_INDEX int = 8
var FLYING_INDEX int = 9
var PSYCHIC_INDEX int = 10
var BUG_INDEX int = 11
var ROCK_INDEX int = 12
var GHOST_INDEX int = 13
var DRAGON_INDEX int = 14
var DARK_INDEX int = 15
var STEEL_INDEX int = 16
var FAIRY_INDEX int = 17
var NUM_PLAYERS int = 2
var IMMUNE int = 0
var RESIST int = 1
var SUPER_EFFECTIVE int = 3

/*
Remember, no magic numbers
instead:
var EXAMPLE_STATIC int = 0
*/

func readmatchup(userPkmn string,botPkmn int)([]int){
	matchupfile, err := os.Open(userPkmn + ".csv")
	if err != nil  {
		log.Fatal(err)
	}
	defer matchupfile.Close()
	read := csv.NewReader(matchupfile)
	matchups, err := read.ReadAll()
	if err != nil {
		log.Fatal(err)
	}
	//fmt.Print(matchups)
	matchup := make([]int, NUM_PLAYERS)
	for i := range matchups[botPkmn]{
		matchup[i], err = strconv.Atoi(matchups[botPkmn][i])
		if err != nil {
			log.Fatal(err)
		}
	}
	return matchup
}

func pickwin(userPkmn string, botPkmn int){
	var matchup []int = readmatchup(userPkmn, botPkmn)
	
	var opponentEffectiveness int = matchup[0]
	var playerEffectiveness int = matchup[1]
	
	
	switch opponentEffectiveness{
	case IMMUNE:
		fmt.Println("Your Pokemon is immune to the attack!")
	case RESIST:
		fmt.Println("Your Pokemon resisted the attack!")
	case SUPER_EFFECTIVE:
		fmt.Println("The opponent's attack is super effective!")
	default:
		fmt.Println("The opponent's attack hits!")
	}
	switch playerEffectiveness{
	case IMMUNE:
		fmt.Println("The opponent is immune to your attack!")
	case RESIST:
		fmt.Println("The opponent resisted your attack!")
	case SUPER_EFFECTIVE:
		fmt.Println("Your attack is super effective!")
	default:
		fmt.Println("Your attack hits!")
	}
	switch {
	case opponentEffectiveness > playerEffectiveness:
		fmt.Println("Your Pokemon has fainted. You lose...")
	case opponentEffectiveness < playerEffectiveness:
		fmt.Println("Your opponent's pokemon has fainted. You win!")
	default:
		fmt.Println("Both Pokemon are evenly matched. It's a draw!")
	}
	
	
}

func main(){
	rand.Seed(time.Now().UnixNano())
	fmt.Println("Please enter a pokemon type through gen 8.")
	var userPkmn string
	fmt.Scanln(&userPkmn)
	strings.ToLower(userPkmn)
	botPkmn := rand.Intn(NUM_PKMN_TYPES)
	//pickwin(userPkmn, botPkmn)
	fmt.Println("The bot picked "+pkmnList[botPkmn])
	pickwin(userPkmn, botPkmn)
	
}

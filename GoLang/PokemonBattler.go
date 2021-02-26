package main

import(
	//"os"
	"time"
	"fmt"
	"strings"
	"math/rand" //for computer choice
	//"encoding/csv" //for reading csv files
)
var NUM_PKMN_TYPES int = 18
/*
Remember, no magic numbers
instead:
var EXAMPLE_STATIC int = 0

func readtolist(userPkmn string){

}
func pickwin(userPkmn string, botPkmn int){
	switch userPkmn {
	case "exit":
		os.Exit()
	case "bug":
		//read bug file and check matchup
	case "dark":
		//read dark file and check matchup
	case "dragon":
		//read dragon file and check matchup
	case "electric":
		//read electric file and check matchup
	case "fairy":
		//read fairy file and check matchup
	case "fighting":
		//read fighting file and check matchup
	case "fire":
		//read fire file and check matchup
	case "flying":
		//read flying file and check matchup
	case "ghost":
		//read ghost file and check matchup
	case "grass":
		//read grass file and check matchup
	case "ground":
		//read ground file and check matchup
	case "ice":
		//read ice file and check matchup
	case "normal":
		//read normal file and check matchup
	case "poison":
		//read poison file and check matchup
	case "psychic":
		//read psychic file and check matchup
	case "rock":
		//read rock file and check matchup
	case "steel":
		//read steel file and check matchup
	case "water":
		//read water file and check matchup
	default:
		fmt.Println("Please enter a valid pokemon type")
		fmt.Scanln(&userPkmn)
	}
}
*/
func main(){
	rand.Seed(time.Now().UnixNano())
	fmt.Println("Please enter a pokemon type through gen 8.")
	var userPkmn string
	fmt.Scanln(&userPkmn)
	strings.ToLower(userPkmn)
	pkmnList := [18]string{"bug","dark","dragon","electric","fairy","fighting","fire","flying","ghost","grass","ground","ice","normal","poison","psychic","rock","steel","water"}
	botPkmn := rand.Intn(NUM_PKMN_TYPES)
	//pickwin(userPkmn, botPkmn)
	fmt.Println("The bot picked "+pkmnList[botPkmn])
	
	//I plan to turn this switch into a function called pickwin
}
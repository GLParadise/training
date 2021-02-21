import random

if __name__=='__main__':
	def determineWinner(compChoice, playChoice, wins):
		if playChoice == compChoice:
			print("It's a draw. Want to play again?")
			return wins
		elif (compChoice == 1 and playChoice == 2) or (compChoice == 2 and playChoice == 3) or (compChoice == 3 and playChoice == 1):
			print("You win!")
			wins[0]+=1
			return wins
		else:
			print("Drats, you lose...")
			wins[1]+=1
			return wins

	rps = ["rock", "paper", "scissors"]
	wins = [0,0]
	while True:
		playerChoice = input("Let's play rock paper scissors, make your choice! ")
		playerChoice.lower()
		computerChoiceInt = random.randint(1,3)
		computerChoice = rps[computerChoiceInt-1]
		print(playerChoice)
		if playerChoice != rps[0] and playerChoice != rps[1] and playerChoice != rps[2]:
			print("Please choose from rock, paper or scissors!")
			continue
		print("Computer chooses",computerChoice)
		wins = determineWinner(computerChoiceInt, rps.index(playerChoice)+1, wins)
		if wins[1] == 3:
			print("The computer has won the best 3 out of 5. Better luck next time!")
			break
		if wins[0] == 3:
			print("You won the best 3 out of 5! Congratulations!")
			break

	

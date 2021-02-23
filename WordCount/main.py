import csv
#import readwrite
def listOfWords(fileName):
    #the following list is all punctuation, spaces and line breaks that would not be part of a word or would mark the end of a word
    wordEnd = ['.', ' ', ',', ':', ';', '"', '!', '?', ')', '\n', '(', '/', '|', '~', '<', '>']
    
    #open the file to read it
    testFile = open(fileName+'.txt', 'r')
    
    #read the file
    fileText = testFile.read()
    
    #creates an empty list to store the words from the text file
    wordList=['']
    
    #boolean that checks if the most recent character was an exception character
    previousWasWordEnd = False
    
    #this loop goes through the characters of the file, checks for exception characters to mark the end of words, and results in a list in which each 
    #index location is a separate word in the order it appears in the file
    for letter in fileText:
        if wordEnd.count(letter)>0:
            #checks for exception character. If it is at the end of a word, itadds an additional index for the next word
            if not previousWasWordEnd:
                wordList.append('')
            previousWasWordEnd = True
        else:
            #adds the current letter to the end of the last word in the list
            wordList[len(wordList)-1]+=letter
            previousWasWordEnd = False
    if previousWasWordEnd:
        #if the last character of the file is an exception character, deletes the blank index at the end of the word list
        wordList.pop()
    for word in range(len(wordList)):
        #converts all strings to lowercase so word counting will be case insensitive
        wordList[word]=wordList[word].lower()
    return wordList

def countWords(wordList):
    #generate a nested list in which the first list is a non-duplicating list of words, and the second is the corresponding count of the words
    wordCount = [[],[]]
    for word in wordList:
        #if the word is not a duplicate, add it to the list of words, then puts the count in the corresponding index of the 2nd list
        if wordCount[0].count(word)==0:
            wordCount[0].append(word)
            wordCount[1].append(wordList.count(word))
    return wordCount

if __name__ == "__main__":
    #ask for the name of the text file the user wants to check the word count of
    fileName = input("What is the name of the .txt file you would like me to scan?")
    
    #call the defined function that turns a .txt file into a list of words in it
    wordList = listOfWords(fileName)
    #call the defined function that counts the number of each word
    wordCount = countWords(wordList)
    indexLocation = 0
    for word in wordCount[0]:
        print(word+",",wordCount[1][indexLocation])
        indexLocation+=1
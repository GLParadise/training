import csv

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

def countSorter(wordCount):
    #runs an insertion sort on the wordCount list would consider changing sort type if this was intended for documents with
    #more different words
    for count in range(1, len(wordCount[1])): 
        #keys to hold the values of the two lists in the nest
        keyCount = wordCount[1][count]
        keyWord = wordCount[0][count] 
        scanner = count-1

        while scanner >= 0 and keyCount > wordCount[1][scanner] :
                #moves the scanned index up if the number at the current index is greater than the number at the scanned index
                wordCount[1][scanner + 1] = wordCount[1][scanner]
                wordCount[0][scanner + 1] = wordCount[0][scanner]
                scanner -= 1
        #puts current value at the lowest index that it should exist in
        wordCount[1][scanner + 1] = keyCount
        wordCount[0][scanner+1] = keyWord
    return wordCount

def writeToDoc(sortedList, fileName):
    #a shortcut function to write the sorted list in the desired format
    #keeps main readable
    with open(fileName+'.csv', 'w', newline='') as csvfile:
        docWriter = csv.writer(csvfile, delimiter=',', quotechar='|', quoting=csv.QUOTE_MINIMAL)
        for index in range(len(sortedList[0])):
            docWriter.writerow([sortedList[0][index]]+[sortedList[1][index]])

if __name__ == "__main__":
    #ask for the name of the text file the user wants to check the word count of
    fileName = input("What is the name of the .txt file you would like me to scan? Enter exit to quit.")
    if fileName.lower() == 'exit':
        quit()
    
    #call the defined function that turns a .txt file into a list of words in it
    wordList = listOfWords(fileName)
    #call the defined function that counts the number of each word
    wordCount = countWords(wordList)
    #call the defined function that sorts the word count
    sortedCount = countSorter(wordCount)
    #ask what the user wants to save the .csv file as
    saveFile = input("What would you like to name the sorted .csv file? Enter exit to quit.")
    if saveFile.lower() == 'exit':
        quit()
    writeToDoc(sortedCount, saveFile)
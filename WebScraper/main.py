import requests
from bs4 import BeautifulSoup

if __name__ == '__main__':
    f = open("scrape.csv", "w")

    url="https://www.google.com/search?q=penguin&rlz=1C1CHBF_enUS779US779&oq=penguin&aqs=chrome.0.69i59l2j0i67j0i67i433j46i131i433j46i67i131i433j46i131i433j0i67.1728j0j7&sourceid=chrome&ie=UTF-8"
    page = requests.get(url)

    soup = BeautifulSoup(page.content, 'html.parser')
    #f.write(soup.prettify())
    
    results = soup.find_all('span', class_= "rQMQod Xb5VRe")
    for search_results in results:
        f.write(search_results.text.strip()+"\n")
    print('done')
# WikiDataHistory
Collect top events in World History from WikiData into a local SQLite database.
In my effort to create an immersive Family History experience, I wanted to add additional context to a family member's timeline.
I created a SPARQL query to collect top events for a given year.
This Dot Net Console application iterates on this SPARQL query for all the desired years.
WikiData's https://query.wikidata.org endpoint is called directly to collect this information.
The resulting Json data is stored in a local SQLite database.
I was able to collect top events from the year 1200 to present (2022) in under 30 minutes!

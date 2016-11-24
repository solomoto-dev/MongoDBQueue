# MongoDBQueue
Simple mongodb-based messaging queue. 
Features:
-persisted messaging
-each receiver gets his own copy of message
-each processing step is atomic and traceable
-messages that weren`t processed successfully are being resent
-IoC independent

#Demo applications
We have two versions - for current and legacy versions of package

#Tests
To run tests you need working MongoDB on mongodb://localhost:27017

#Nearest plans:
1) create user-friendly initializer (probably fluent)
2) recreation of Envelops collection for a route if it`s been deleted when application is running
3) timestamp to see when subscription was created last timestamp
4) configurable listener - add a possibility to choose cursor type, tailable or polling




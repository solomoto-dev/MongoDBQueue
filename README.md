# MongoDBQueue
Simple mongodb-based messaging queue. Current implementation uses polling instead of tailable cursors.

#Features:
- persisted messaging
- each receiver gets his own copy of message
- each processing step is atomic and traceable
- messages that weren`t processed successfully are being resent
- IoC agnostic



#Demo applications
We have two versions - for current and legacy versions of package

#Tests
To run tests you need working MongoDB on mongodb://localhost:27017

#Future plans:
- recreation of Envelops collection for a route if it`s been deleted when application is running
- configurable listener - add a possibility to choose cursor type, tailable or polling




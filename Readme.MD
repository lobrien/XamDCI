# Data-Context-Interaction Example

This project shows the use of [Data-Context-Interaction](http://fulloo.info/) in a Xamarin.iOS MVC application. 

## WARNING: Over-Engineered!

This project is intended to illustrate techniques that are only *actually* applicable in far-more-complex situations.
The application is over-engineered in terms of MVC and DCI is not called for in use-cases that can be implemented with 
a handful of lines. 

## WARNING: Under-Engineered!

This project's UX is intentionally terrible. 

## Purpose

The purpose of this project is to illustrate two techniques that are helpful in larger applications: 

- MVC with domain events; and
- DCI

This project illustrates a "stricter" interpretation of MVC (as opposed to MVP and without the use of MVVM). 
Domain events (in this case, relating to the transfer of money) are used to decouple View and Controller elements 
from the Model. Additionally, View elements directly subscribe to these domain events, thus distinguishing this 
architecture from MVP. (For more discussion, see [article](tk)). 

DCI is a technique that is useful in complex use-cases, where the algorithm or business logic is a
matter of refinement, validation by domain experts, etc. (In other words, situations more complex than what's 
strictly needed for this example.) This example shows the basic DCI structure:

- The Controller establishes a Context, which corresponds to a use-case
- The Controller creates "role-playing objects" by injecting methods ("what objects do" vs. "what objects are").
- The use-case is defined by the explicit interaction of role-playing objects 

Because C# does not have language support for Role types, Roles are defined as interfaces and injected via
extension methods. 


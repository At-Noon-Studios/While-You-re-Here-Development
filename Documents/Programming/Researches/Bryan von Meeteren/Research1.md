# While-You-re-Here-Development

# Research about the inventory system (won't be implemented)

- [[#Fixed Inventory|Fixed Inventory]]
- [[#Slot-Based Inventory (Array)|Slot-Based Inventory (Array)]]
- [[#Dynamic Inventory (Lists)|Dynamic Inventory (Lists)]]
- [[#Making Items|Making Items]]
- [[#Do the items have to actually exist in the game world?|Do the items have to actually exist in the game world?]]
- [[#Using Unity UI or UI-Toolkit?|Using Unity UI or UI-Toolkit?]]
- [[#Potential questions for the gameplay team:|Potential questions for the gameplay team:]]


[This video](https://www.youtube.com/watch?v=pmBv0Cagx_o&list=WL) gives a beginner-friendly introduction about the 3 most common ways to design an inventory system. It helps you think about what kind of system you'd like to implement for your own game:.

Generally, there are 3 different types of inventory systems:
- Fixed inventory, variable-based systems, uses booleans.
- Slot-based inventory, assign items to specific positions, uses arrays.
- Dynamic Inventory, variable in size, can easily be sorted, uses lists.

If we want to implement an inventory system where some slots are specifically left empty, we would need to use a slot-based inventory system which uses an array, since lists can't have empty elements.
### Fixed Inventory
**Pros**:
- Limited number of items which you either have or not (booleans)
- Other classes can then check against this inventory class by referencing it 
- Simple, relatively extendable
- Easy interaction
**Cons:**
- Only suitable if the contents of the inventory are known in advance
### Slot-Based Inventory (Array)
- Length is specified in advance
### Dynamic Inventory (Lists)
- List has a variable size, which is defined by their contents and can be modified during runtime

### Making Items

You can create items in different ways to store them/their data
- Enums:
	- By creating a list of items in an enum you can make it so the state of having the item is the selection in the dropdown
- Plain Class
	- Reference type, so it can be null if you want some slots to be empty, you have to use a class
- Structs
	- Value type so it can't be null 
- Scriptable Objects
	- Use Scriptable Objects for data about items that doesn't need to be changed, like name or description. SO's are useful since they can be made ahead of time. 
	- These can also be used on their own to create an inventory system, the state of having an item could be the presence of a Scriptable Object.

### Do the items have to actually exist in the game world?
If your items spend most of their time as real objects -> gameObject-based inventory 
Otherwise consider data-structures for items


### Using Unity UI or UI-Toolkit?

https://docs.unity3d.com/6000.0/Documentation/Manual/UI-system-compare.html

The inventory system will be realised with UI-Toolkit, this is also easier to understand and useful for non-programmers, and it was used during the first part of the minor, as a research subject.

### Potential questions for the gameplay team:
(Maybe they just answer these by themselves in sprint 2)
1. Which data does every item inside the inventory need? (name, icon, description, count, this will be used to create the scriptable objects)
2. Does the data ever change? (status effect, durability)
3. Can Nora hold more than 1 item of the same kind?
4. Can Nora discard items and if so, what happens to this item?
5. How many unique items are there in the game?
6. Can her inventory be _'full'_?
7. Is Nora's inventory filled with items or is okay to have empty slots?
8. Can the player rearrange Nora's items inside the inventory, so drag them to different slots?
9. Does the inventory need to be sorted, or be able to be sorted?

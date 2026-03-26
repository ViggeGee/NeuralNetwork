# NeuralNetwork Simulation

This was a school project where I built a simulation in Unity where an agent learns to find food on their own, without any manual programming of their behavior.

The agent has a small neural network as its brain. It takes in information about its surroundings through raycasts. Based on that, the network decides how to move.

The agent isn't trained in a traditional sense. Instead, when one dies its weights are copied to a new agent and slightly mutated. Over time, the ones that happen to find food live longer and pass on better weights. It's loosely based on how evolution works.

I also built a small debug UI that lets you speed up the simulation, toggle visibility of dead agents, and see how many generations have passed.

## How it works

- Agents sense their environment using 9 raycasts
- The distances are fed into a neural network (9 → 16 → 8 → 2)
- The network outputs two values: forward/backward and left/right
- Agents lose energy over time and die if they don't find food
- When an agent dies, a new one is spawned with a mutated copy of its neural network
- If an agent performs poorly several times in a row, the mutation rate increases to escape bad local solutions

## Built with

- Unity
- C#

## TODO

- Add elitism to the network
- Introduce obstacles to the network
- (Maybe) Adjust the spawning of the food to be placed in a more consistent manner

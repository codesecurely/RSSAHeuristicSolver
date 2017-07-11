# RSAHeuristicSolver

This is not a RSA asymmetrical encryption algorithm cracker or anything of this sort. The RSA problem concerns routing and spectrum allocation in elastic optical networks, which means assignment of frequency resources, divided into 12.5 GHz spectrum slices grouped into channels and routing paths, i.e. sequences of network links to either anycast or unicast demands. The goal is to minimize the necessary spectrum to cover all demands.

This project expands the RSA problem in the spatial domain. Demands may now be allocated on any of spatial resources (more or less optical links, some sort of fiber) on a given path. This problem is NP-hard and requires heuristic approach to large instances.

This simulator implements a greedy heuristic algorithm and metaheuristic simulated annealing algorithm to solve the RSSA - routing, spectrum and space allocation problem.

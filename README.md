# Vibe coding a Godot game with Antigravity and Gemini Pro (High)

* Had to create the initial Godot project
* Iteration is a pretty high monetary cost
  * It gets things wrong a lot, requiring a lot of back and forth
* Not great without verification tooling
  * eg. verifying that something looks correct or that a scene is not broken
* Not great without a domain expert
  * Hard to help it with shaders and such without knowing how to tell it what to fix
* Good at "I know how to do this, just write the code quickly for me"
* Not good at maintaining consistent code practices
  * eg. Godot namespace is global in this project but it keeps importing it in every new source file despite being told to stop doing that
* It keeps wanting run `dotnet build` even when no .cs file has changed
* Not great recovery when internet drops

using UnityEngine;
using TMPro;
using System.Collections.Generic;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using System.Linq;

public class CommandsHandler : MonoBehaviour
{

    public TMP_Text terminal_text;
    public TMP_InputField terminal_input;
    public GameObject BedroomOverlay;
    public GameObject BathroomOverlay;
    public GameObject KitchenOverlay;
    public GameObject OfficeOverlay;
    public GameObject GarageOverlay;

    public GameObject BedroomDoor;
    public GameObject GarageDoor;
    public GameObject KitchenDoor;

    public GameObject Sprinkler;

    public GameObject Stereo;

    Queue<string> commands = new Queue<string>();
    public int queue_pointer = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        terminal_input = this.GetComponent<TMP_InputField>();
    }

    // Update is called once per frame
    void Update()
    {
        bool pointer_changed = false;
        if (UnityEngine.InputSystem.Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            if (queue_pointer - 1 < commands.Count && queue_pointer - 1 >= 0)
            {
                queue_pointer--;
                pointer_changed = true;
            }
        }
        if (UnityEngine.InputSystem.Keyboard.current.downArrowKey.wasPressedThisFrame)
        {
            if (queue_pointer + 1 <= commands.Count && queue_pointer + 1 >= 0)
            {
                queue_pointer++;
                pointer_changed = true;
            }
        }
        if (pointer_changed)
        {
            if (queue_pointer == commands.Count)
                terminal_input.text = "";
            else
                terminal_input.text = commands.ElementAt(queue_pointer);
        }
    }

    bool processCommand(string command)
    {
        // Implement your command processing logic here
        // Return true if the command was processed successfully, false otherwise

        if (command.Contains("help"))
        {
            terminal_text.text += "\nAvailable commands:\n- <color=#00FFFF>help</color>: Show this help message\n- <color=#00FFFF>clear</color>: Clear the terminal\n- <color=#00FFFF>echo [message]</color>: Echo the message back to the terminal \n- <color=#00FFFF>room.lights.on/off</color>: Turn on/off room lights \n- <color=#00FFFF>room.door.open/close</color>: Open/close room doors";
            return true;
        }
        else if (command.Contains("clear"))
        {
            terminal_text.text = "";
            return true;
        }
        else if (command.StartsWith("echo "))
        {
            string message = command.Substring(5); // Get the message after "echo "
            terminal_text.text += "\n" + message;
            return true;
        }
        else if (command.Contains("bedroom.lights.on"))
        {
            terminal_text.text += "\nTurning on bedroom lights...";
            BedroomOverlay.SetActive(false);
            // Add your logic to turn on the bedroom lights here
            return true;
        }
        else if (command.Contains("bedroom.lights.off"))
        {
            terminal_text.text += "\nTurning off bedroom lights...";
            BedroomOverlay.SetActive(true);
            // Add your logic to turn off the bedroom lights here
            return true;
        }
        else if (command.Contains("bathroom.lights.on"))
        {
            terminal_text.text += "\nTurning on bathroom lights...";
            BathroomOverlay.SetActive(false);
            // Add your logic to turn on the bathroom lights here
            return true;
        }
        else if (command.Contains("bathroom.lights.off"))
        {
            terminal_text.text += "\nTurning off bathroom lights...";
            BathroomOverlay.SetActive(true);
            // Add your logic to turn off the bathroom lights here
            return true;
        }
        else if (command.Contains("kitchen.lights.on"))
        {
            terminal_text.text += "\nTurning on kitchen lights...";
            KitchenOverlay.SetActive(false);
            // Add your logic to turn on the kitchen lights here
            return true;
        }
        else if (command.Contains("kitchen.lights.off"))
        {
            terminal_text.text += "\nTurning off kitchen lights...";
            KitchenOverlay.SetActive(true);
            // Add your logic to turn off the kitchen lights here
            return true;
        }
        else if (command.Contains("office.lights.on"))
        {
            terminal_text.text += "\nTurning on office lights...";
            OfficeOverlay.SetActive(false);
            // Add your logic to turn on the office lights here
            return true;
        }
        else if (command.Contains("office.lights.off"))
        {
            terminal_text.text += "\nTurning off office lights...";
            OfficeOverlay.SetActive(true);
            // Add your logic to turn off the office lights here
            return true;
        }
        else if (command.Contains("garage.lights.on"))
        {
            terminal_text.text += "\nTurning on garage lights...";
            GarageOverlay.SetActive(false);
            // Add your logic to turn on the garage lights here
            return true;
        }
        else if (command.Contains("garage.lights.off"))
        {
            terminal_text.text += "\nTurning off garage lights...";
            GarageOverlay.SetActive(true);
            // Add your logic to turn off the garage lights here
            return true;
        }
        else if (command.Contains("bedroom.door.open"))
        {
            terminal_text.text += "\nOpening bedroom door...";
            BedroomDoor.SetActive(true);
            // Add your logic to open the bedroom door here
            return true;
        }
        else if (command.Contains("bedroom.door.close"))
        {
            terminal_text.text += "\nClosing bedroom door...";
            BedroomDoor.SetActive(false);
            // Add your logic to close the bedroom door here
            return true;
        }
        else if (command.Contains("garage.door.open"))
        {
            terminal_text.text += "\nOpening garage door...";
            GarageDoor.SetActive(true);
            // Add your logic to open the garage door here
            return true;
        }
        else if (command.Contains("garage.door.close"))
        {
            terminal_text.text += "\nClosing garage door...";
            GarageDoor.SetActive(false);
            // Add your logic to close the garage door here
            return true;
        }
        else if (command.Contains("kitchen.door.open"))
        {
            terminal_text.text += "\nOpening kitchen door...";
            KitchenDoor.SetActive(true);
            // Add your logic to open the kitchen door here
            return true;
        }
        else if (command.Contains("kitchen.door.close"))
        {
            terminal_text.text += "\nClosing kitchen door...";
            KitchenDoor.SetActive(false);
            // Add your logic to close the kitchen door here
            return true;
        }
        else if (command.Contains("garden.sprinkler.on"))
        {
            terminal_text.text += "\nStarting garden sprinkler...";
            Sprinkler.GetComponent<Animator>().SetBool("watering", true);
            return true;
        }
        else if (command.Contains("garden.sprinkler.off"))
        {
            terminal_text.text += "\nStoping garden sprinkler...";
            Sprinkler.GetComponent<Animator>().SetBool("watering", false);
            return true;
        }
        else if (command.Contains("bedroom.stereo.play(metal)")){
            terminal_text.text += "\nPlaying metal...";
            Stereo.GetComponent<PlayingStereo>().playMetal();
            return true;
        }
        else if (command.Contains("bedroom.stereo.play(mozart)"))
        {
            terminal_text.text += "\nPlaying mozart...";
            Stereo.GetComponent<PlayingStereo>().playMozart();
            return true;
        }
        else if (command.Contains("bedroom.stereo.off")){
            terminal_text.text += "\nTurning off radio...";
            Stereo.GetComponent<PlayingStereo>().stopMusic();
            return true;
        }


        terminal_text.text += "\nUnknown command: <color=red>\"" + command + "\"</color>";
        return false;
    }
    public void receiveCommand()
    {
        var command = gameObject.GetComponent<TMP_InputField>().text;
        gameObject.GetComponent<TMP_InputField>().text = "";
        terminal_text.text += "\n> " + command;
        commands.Enqueue(command);
        queue_pointer = commands.Count;
        processCommand(command);
        Debug.Log("Received command: " + command);
        // Process the command here
        StartCoroutine(Refocus());
    }

    System.Collections.IEnumerator Refocus()
    {
        yield return null; // wait 1 frame
        gameObject.GetComponent<TMP_InputField>().ActivateInputField();
        gameObject.GetComponent<TMP_InputField>().Select();
    }
}

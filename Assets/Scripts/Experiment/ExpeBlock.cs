using System;

public class ExpeBlock
{

    public int blockID { get; set; }
    public TI ti { get; set; }
    public Visualization visualization { get; set; }
    public Task task { get; set; }

    public int numberOfObjects { get; set; }

    public int trainingTrialNum { get; set; }

    public int monitoredTrialNum { get; set; }

    public int startingTrial{ get; set; }

    public int currentTrial{ get; set; }

    public ExpeBlock(
        int blockID_,
        TI ti_, 
        Visualization visualization_, 
        Task task_,
        int numberOfObjects_,
        int trainingTrialNum_,
        int monitoredTrialNum_,
        int startingTrial_){
        blockID = blockID_;
        ti = ti_; visualization = visualization_; task = task_; numberOfObjects = numberOfObjects_;
        trainingTrialNum = trainingTrialNum_;
        monitoredTrialNum = monitoredTrialNum_;
        startingTrial = startingTrial_;
        currentTrial = startingTrial;
    }

    public static ExpeBlock FromString(
        int blockID,
        string tiString, 
        string visualizationString,
        string taskString, 
        string numberOfObjectsString,
        int trainingTrialNum, 
        int monitoredTrialNum,
        int startingTrial){

        var numberOfObjects = 200;
        try{
            numberOfObjects = Int32.Parse(numberOfObjectsString);
        }
        catch (FormatException)
        {
            throw new System.Exception($"Couldn't format numberOfObjects {numberOfObjectsString} in int");
        }
        var ti = TIMethods.FromString(tiString);
        var visualization = VisualizationMethods.FromString(visualizationString);
        var task = TaskMethods.FromString(taskString);

        if (ti == TI.Undifined || visualization == Visualization.Undifined || task == Task.Undifined){
            throw new System.Exception($"{tiString} or {visualizationString} or {taskString} not matching");
        }

        return new ExpeBlock(blockID, ti, visualization, task, numberOfObjects, trainingTrialNum, monitoredTrialNum, startingTrial);
    }

    public static ExpeBlock FromBlockChannel(BlockChannel blockChannel){
        Block_ blockData = blockChannel.block;
        return ExpeBlock.FromString(
            blockData.id, 
            blockData.@params.TI, 
            blockData.@params.Visualization,
            blockData.@params.Task,
            blockData.@params.NumberOfObjects,
            blockData.trainingTrialsCount,
            blockData.monitoredTrialsCount,
            blockData.currentTrial);
    }

    public ExpeTrial generateNextTrial(){
        ExpeTrial trial = null;
        if(currentTrial < trainingTrialNum + monitoredTrialNum){
            if(currentTrial >= trainingTrialNum){
                trial = new ExpeTrial(currentTrial, false);
            }
            else{
                trial = new ExpeTrial(currentTrial, true);
            }
            currentTrial ++;
        }

        return trial;
    }
}


// Parametre Technique d'interaction
public enum TI{
    AB, OrthozoomDepthJoystick, OrthozoomDepth, OrthozoomHeight, OrthozoomJoystickControllerDist, OrthozoomJoystickJoystick, Slider, Undifined
}

// Parametre Visalisation
public enum Visualization{
    Circle, Helice, PerspectiveWall, NoTimeline, Undifined
}

// Tache 
public enum Task{
    Locate, Pattern, Undifined
}

static class TIMethods{
    public static TI FromString(string tiString){
        switch(tiString){
            case "AB": return TI.AB;
            case "OrthozoomDepthJoystick": return TI.OrthozoomDepthJoystick;
            case "OrthozoomDepth": return TI.OrthozoomDepth;
            case "OrthozoomHeight": return TI.OrthozoomHeight;
            case "OrthozoomJoystickControllerDist": return TI.OrthozoomJoystickControllerDist;
            case "OrthozoomJoystickJoystick": return TI.OrthozoomJoystickJoystick;
            case "Slider": return TI.Slider;
            default: return TI.Undifined;
        }
    }

    public static string ToString(this TI ti){
        switch(ti){
            case TI.AB: return "AB";
            case TI.OrthozoomDepthJoystick: return "OrthozoomDepthJoystick";
            case TI.OrthozoomDepth: return "OrthozoomDepth";
            case TI.OrthozoomHeight: return "OrthozoomHeight";
            case TI.OrthozoomJoystickControllerDist: return "OrthozoomJoystickControllerDist";
            case TI.OrthozoomJoystickJoystick: return "OrthozoomJoystickJoystick";
            case TI.Slider: return "Slider";
            default: return "Undifined";
        }
    }
}

static class VisualizationMethods{
    public static Visualization FromString(string visualizationString){
        switch(visualizationString){
            case "Circle": return Visualization.Circle;
            case "Helice": return Visualization.Helice;
            case "PerspectiveWall": return Visualization.PerspectiveWall;
            case "NoTimeline": return Visualization.NoTimeline;
            default: return Visualization.Undifined;
        }
    }

    public static string ToString(this Visualization visualization){
        switch(visualization){
            case Visualization.Circle: return "Circle";
            case Visualization.Helice: return "Helice";
            case Visualization.PerspectiveWall: return "PerspectiveWall";
            case Visualization.NoTimeline: return "NoTimeline";
            default: return "Undifined";
        }
    }
}

static class TaskMethods{
    public static Task FromString(string taskString){
        switch(taskString){
            case "Locate": return Task.Locate;
            case "Pattern": return Task.Pattern;
            default: return Task.Undifined;
        }
    }

    public static string ToString(this Task task){
        switch(task){
            case Task.Locate: return "Locate";
            case Task.Pattern: return "Pattern";
            default: return "Undifined";
        }
    }
}
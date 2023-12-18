using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class FullLogger
{

    const string columnSignature = (
        "user_ID;"+ 
        "group_ID;" + 
        "block_ID;" + 
        "visu;" + 
        "ti;" + 
        "task;" + 
        "trial_number;" + 
        "is_training;" + 
        "number_of_object;" +
        "current_phase;" + 
        "start_pos;" + 
        "current_pos;" + 
        "target_pos;" + 
        "distance;" + 
        "left_Right;" + 
        "number_of_pattern_to_find;" + 
        "pattern_type;" +
        "number_of_pattern_found;" +
        "number_of_pattern_not_found;" +
        "did_find_all_patterns;" +
        "pattern1_pos;" +
        "pattern2_pos;" +
        "pattern3_pos;" +
        "pattern4_pos;" +
        "pattern5_pos;" +
        "first_pattern_found_pos;" +
        "second_pattern_found_pos;" +
        "third_pattern_found_pos;" +
        "fourth_pattern_found_pos;" +
        "fifth_pattern_found_pos;" +
        "current_time;" + 
        "total_time;" + 
        "time_before_first_move;" + 
        "time_grab;" + 
        "time_for_final_selection;" + 
        "time_to_validate_pattern1;" + 
        "time_to_validate_pattern2;" + 
        "time_to_validate_pattern3;" + 
        "time_to_validate_pattern4;" +
        " +time_to_validate_pattern5;" + 
        "timeAfterLastSelection;" + 
        "time_using_interaction;" + 
        "number_of_retrieve;" + 
        "number_of_bad_select;" + 
        "number_of_click_inside_scrollbar_handle;" + 
        "number_of_click_outside_scrollbar_handle;" +
        "number_of_click_on_plus;" +
        "number_of_click_on_minus;" +
        "number_of_click_on_a;" +
        "number_of_click_on_b;" +
        "number_of_click_on_both_button;" +
        "current_head_amplitude;" + 
        "max_head_amplitude;" + 
        "current_head_rotation.x;" + 
        "current_head_rotation.y;" + 
        "current_head_rotation.z;" +
        "current_zoom;" + 
        "current_ortho_distance;" + 
        "current_cd_gain;" + 
        "current_controller_distance;" + 
        "current_joystick_x;"
    );
    
    string logFileName;

    string logFullPath;

    int lineNumber = 0;

    List<string> logLines = new List<string>();
    
    // Start is called before the first frame update

    void CreateFile(int userID){
        string expeLogsDirectory = Application.persistentDataPath + "/expeLogs";
        string resumeLogsDirectory = expeLogsDirectory + "/fullLogs";
        //C:\Users\amarchal\AppData\LocalLow\IRIT\ThreeDplusT_Expe\expeLogs\fullLogs\
        //Ce PC\Quest Pro\Espace de stockage interne partagé\Android\data\com.IRIT.ThreeDplusT_Expe\files\expeLogs\fullLogs\
        logFileName = $"full_user{userID}.csv";
        logFullPath = resumeLogsDirectory + "/" + logFileName;
        if(!Directory.Exists(expeLogsDirectory)){
            Directory.CreateDirectory(expeLogsDirectory);
        }

        if(!Directory.Exists(resumeLogsDirectory)){
            Directory.CreateDirectory(resumeLogsDirectory);
        }


        if(!File.Exists(logFullPath)){
            File.Create(logFullPath).Dispose();
            
            using(var sw = new StreamWriter(logFullPath, true))
            {
                sw.WriteLine(columnSignature);
            }
        }
        else{
            lineNumber = File.ReadAllLines(logFullPath).Length - 1;
        }
    }

    private void StoreLine(string logLine){
        logLines.Add(logLine);
    }

    public void WriteLogLines(int userID, float totalTime, float time_after_last_selection, float max_head_amplitude){
        CreateFile(userID);
        //Debug.Log("lineNumber, " +  logLine);
        using(var sw = new StreamWriter(logFullPath, true)){
            foreach(string logLine in logLines){
                string line = logLine.Replace("total_time", $"{totalTime}");
                line = line.Replace("max_head_amplitude", $"{max_head_amplitude}");
                line = line.Replace("time_after_last_selection", $"{time_after_last_selection}");
                sw.WriteLine(line);
            }
        }

        logLines.Clear();
    }

    public void LogTimeStamp(
        int user_ID, 
        int group_ID, 
        int block_ID, 
        string visu,
        string ti,
        string task,
        int trial_number,
        bool is_training,
        int number_of_object,
        string current_phase,
        int current_pos,
        int start_pos,
        int? target_pos,
        int? distance,
        string left_Right,

        int? number_of_pattern_to_find,
        string pattern_type,
        int? number_of_pattern_found,
        int? number_of_pattern_not_found,
        bool? did_find_all_patterns,
        int? pattern1_pos,
        int? pattern2_pos,
        int? pattern3_pos,
        int? pattern4_pos,
        int? pattern5_pos,
        int? pattern1_found_pos,
        int? pattern2_found_pos,
        int? pattern3_found_pos,
        int? pattern4_found_pos,
        int? pattern5_found_pos,

        float current_time,
        float time_before_first_move,
        float? time_grab,
        float? time_for_final_selection,
        float? time_to_validate_pattern1,
        float? time_to_validate_pattern2,
        float? time_to_validate_pattern3,
        float? time_to_validate_pattern4,
        float? time_to_validate_pattern5,
        float time_using_interaction,

        int? number_of_retrieve,
        int number_of_bad_select,
        int? number_of_click_inside_scrollbar_handle,
        int? number_of_click_outside_scrollbar_handle,
        int? number_of_click_on_plus,
        int? number_of_click_on_minus,

        int? number_of_click_on_a,
        int? number_of_click_on_b,
        int? number_of_click_on_both_button,

        float current_head_amplitude,
        Vector3 current_head_rotation,

        float? current_zoom,
        float? current_ortho_distance,
        float? current_cd_gain,

        float? current_controller_distance,
        float? current_joystick_x){
        
        StoreLine(
            $"P{user_ID};" + 
            $"{group_ID};" + 
            $"{block_ID};" + 
            $"{visu};" + 
            $"{ti};" + 
            $"{task};" + 
            $"{trial_number};" + 
            $"{is_training};" + 
            $"{number_of_object};" +
            $"{current_phase};" + 
            $"{start_pos};" + 
            $"{current_pos};" + 
            $"{target_pos};" + 
            $"{distance};" + 
            $"{left_Right};" + 
            $"{number_of_pattern_to_find};" + 
            $"{pattern_type};" +
            $"{number_of_pattern_found};" +
            $"{number_of_pattern_not_found};" +
            $"{did_find_all_patterns};" +
            $"{pattern1_pos};" +
            $"{pattern2_pos};" +
            $"{pattern3_pos};" +
            $"{pattern4_pos};" +
            $"{pattern5_pos};" +
            $"{pattern1_found_pos};" +
            $"{pattern2_found_pos};" +
            $"{pattern3_found_pos};" +
            $"{pattern4_found_pos};" +
            $"{pattern5_found_pos};" +
            $"{current_time};" + 
            "total_time;" + 
            $"{time_before_first_move};" + 
            $"{time_grab};" + 
            $"{time_for_final_selection};" + 
            $"{time_to_validate_pattern1};" + 
            $"{time_to_validate_pattern2};" + 
            $"{time_to_validate_pattern3};" + 
            $"{time_to_validate_pattern4};" +
            $"{time_to_validate_pattern5};" + 
            "time_after_last_selection;" + 
            $"{time_using_interaction};" + 
            $"{number_of_retrieve};" + 
            $"{number_of_bad_select};" + 
            $"{number_of_click_inside_scrollbar_handle};" + 
            $"{number_of_click_outside_scrollbar_handle};" + 
            $"{number_of_click_on_plus};" +
            $"{number_of_click_on_minus};" +
            $"{number_of_click_on_a};" +
            $"{number_of_click_on_b};" +
            $"{number_of_click_on_both_button};" +
            $"{current_head_amplitude};" + 
            "max_head_amplitude;" + 
            $"{current_head_rotation.x};" + 
            $"{current_head_rotation.y};" + 
            $"{current_head_rotation.z};" +
            $"{current_zoom};" + 
            $"{current_ortho_distance};" + 
            $"{current_cd_gain};" + 
            $"{current_controller_distance};" + 
            $"{current_joystick_x};"

        );
    }


}

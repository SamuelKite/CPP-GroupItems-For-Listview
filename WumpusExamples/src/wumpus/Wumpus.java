package wumpus;

import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.File;
import java.io.FileReader;
import java.io.FileWriter;
import java.io.IOException;
import javax.swing.filechooser.FileNameExtensionFilter;
import javax.swing.JFileChooser;

public class Wumpus {

  static int[] wumpusCave = null;
  static final int maxRow = 5;
  static final int rowLength = 6;
  static final int maxCave = maxRow * rowLength;
  static final String wumpusCaveFile = "wumpusCave.txt";

  public static class Direction {

    public static final int Top = 0;
    public static final int UpperRight = 1;
    public static final int LowerRight = 2;
    public static final int Bottom = 3;
    public static final int LowerLeft = 4;
    public static final int UpperLeft = 5;
    public static final int MaxDirection = rowLength;

    public static String[] DirectionNames = new String[] {
      "top",
      "upper right",
      "lower right",
      "bottom",
      "lower left",
      "upper left",
    };

    public static String GetDirectionName(int direction) {
      if (direction < 0 || direction >= DirectionNames.length) {
        return "Invalid direction";
      } else {
        return DirectionNames[direction];
      }
    }
  }

  static void LoadWumpusCaveFromFile() {
    JFileChooser fileChooser = new JFileChooser();
    fileChooser.setDialogTitle("Select Wumpus Cave File");
    fileChooser.setFileFilter(new FileNameExtensionFilter("Text Files", "txt"));

    int userSelection = fileChooser.showOpenDialog(null);
    if (userSelection == JFileChooser.APPROVE_OPTION) {
      String filePath = fileChooser.getSelectedFile().getAbsolutePath();
      try (BufferedReader reader = new BufferedReader(new FileReader(filePath))) {
        String line;
        int index = 0;
        while ((line = reader.readLine()) != null) {
          wumpusCave[index] = Integer.parseInt(line);
          index++;
        }
        System.out.println("Wumpus cave loaded successfully.");
      } catch (IOException e) {
        System.out.println("Error loading wumpus cave: " + e.getMessage());
      }
    } else if(userSelection == JFileChooser.CANCEL_OPTION){
      System.out.println("Wumpus cave not loaded. Generating cave.");
      wumpusCave = GenerateWumpusCave();
      SaveWumpusCaveToFile();
    }
  }

  static void SaveWumpusCaveToFile() {
    JFileChooser fileChooser = new JFileChooser();
    fileChooser.setDialogTitle("Save Wumpus Cave File");
    fileChooser.setFileFilter(new FileNameExtensionFilter("Text Files", "txt"));
    fileChooser.setApproveButtonText("Save");
    fileChooser.setSelectedFile(new File(wumpusCaveFile));

    int userSelection = fileChooser.showSaveDialog(null);
    if (userSelection == JFileChooser.APPROVE_OPTION) {
      String filePath = fileChooser.getSelectedFile().getAbsolutePath();
      try (BufferedWriter writer = new BufferedWriter(new FileWriter(filePath))) {
        for (int i = 0; i < wumpusCave.length; i++) {
          writer.write(String.valueOf(wumpusCave[i]));
          writer.newLine();
        }
        System.out.println("Wumpus cave saved successfully.");
      } catch (IOException e) {
        System.out.println("Error saving wumpus cave: " + e.getMessage());
      }
    }
  }

  static int[] GenerateWumpusCave(){
    System.out.println("Initializing wumpus cave...");
    int[] wumpusCave = new int[maxCave * Direction.MaxDirection];
    for (int tile = 0; tile < maxCave; tile++) {
      int tileOffset = tile * 6;
      int tileNumber = tile + 1;
      boolean isOdd = tileNumber % 2 == 1;
      boolean isEven = tileNumber % 2 == 0;
      for (int door = 0; door < Direction.MaxDirection; door++) {
        int destinationTile;
        int newOffset = tileOffset + door;
        switch (door) {
          case Direction.Top:
            destinationTile = tileNumber - rowLength;
            destinationTile =
              destinationTile < 1
                ? maxCave + destinationTile
                : destinationTile;
            wumpusCave[newOffset] = destinationTile;
            break;
          case Direction.UpperRight:
            if (isEven) {
              destinationTile =
                tileNumber % rowLength == 0
                  ? tileNumber - rowLength
                  : tileNumber;
              destinationTile += 1;
              wumpusCave[newOffset] = destinationTile;
            } else if (isOdd) {
              int topTile = wumpusCave[tileOffset + Direction.Top];
              wumpusCave[newOffset] = topTile + 1;
            }
            break;
          case Direction.LowerRight:
            int upperRightDoor =
              wumpusCave[tileOffset + Direction.UpperRight];
            destinationTile = upperRightDoor + rowLength;
            destinationTile =
              destinationTile > maxCave
                ? destinationTile - maxCave
                : destinationTile;
            wumpusCave[newOffset] = destinationTile;
            break;
          case Direction.Bottom:
            destinationTile = tileNumber + rowLength;
            destinationTile =
              destinationTile > maxCave
                ? destinationTile - maxCave
                : destinationTile;
            wumpusCave[newOffset] = destinationTile;
            break;
          case Direction.LowerLeft:
            if (isEven) {
              destinationTile = wumpusCave[tileOffset + Direction.Bottom] - 1;
              destinationTile =
                destinationTile > maxCave
                  ? destinationTile - maxCave
                  : destinationTile;
              wumpusCave[newOffset] = destinationTile;
            } else if (isOdd) {
              destinationTile =
                tileNumber % rowLength == 1
                  ? rowLength + tileNumber
                  : tileNumber;
              destinationTile -= 7;
              destinationTile =
                destinationTile < 1
                  ? rowLength + destinationTile
                  : destinationTile;
              wumpusCave[newOffset] = destinationTile;
            }
            break;
          case Direction.UpperLeft:
          int lowerLeftDoor = wumpusCave[tileOffset + Direction.LowerLeft];
            destinationTile = lowerLeftDoor - rowLength;
            destinationTile =
              destinationTile < 1
                ? maxCave + destinationTile
                : destinationTile;
            wumpusCave[newOffset] = destinationTile;
            break;
        }

        System.out.println(
          "Tile " +
          tileNumber +
          " " +
          Direction.GetDirectionName(door) +
          " door leads to " +
          wumpusCave[newOffset]
        );
      }
    }

    return wumpusCave;
  }

  public static int WumpusMap(int location, int direction) {
    if(location < 1 || location > maxCave){
      System.out.println("Invalid location. Please enter a number between 1 and " + maxCave);
      return 0;
    }

    if (wumpusCave == null)
    {
      LoadWumpusCaveFromFile();
    }

    int currentOffset = (location - 1) * 6;
    if (currentOffset < wumpusCave.length) {
      location = wumpusCave[currentOffset + direction];
    } else {
      System.out.println(
        "You are outside the wumpus cave. This is an error. Please report it to the developer."
      );
    }

    return location;
  }
}

import java.awt.BorderLayout;
import java.awt.Font;
import java.awt.event.KeyEvent;
import java.io.IOException;
import java.io.OutputStream;
import java.io.PrintStream;
import javax.swing.JFrame;
import javax.swing.JScrollPane;
import javax.swing.JTextArea;
import javax.swing.JTextField;
import javax.swing.SwingUtilities;
import wumpus.Wumpus;

public class App {

  static int currentLocation = 1;
  static String userInput = "";



  public static void main(String[] args) {
    SwingUtilities.invokeLater(() -> {
      JFrame frame = new JFrame("Terminal App");
      frame.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
      frame.setSize(400, 300);

      JTextArea terminalArea = new JTextArea();
      terminalArea.setEditable(false); // Make it read-only
      terminalArea.setFont(new Font("Monospaced", Font.PLAIN, 12)); // Use a monospaced font

      // Redirect System.out to the terminal area
      PrintStream originalOut = System.out;
      System.setOut(
        new PrintStream(
          new OutputStream() {
            @Override
            public void write(int b) throws IOException {
              terminalArea.append(String.valueOf((char) b));
              originalOut.write(b);
            }
          }
        )
      );

      // Create an input field
      JTextField inputField = new JTextField();
      inputField.addKeyListener(
        new java.awt.event.KeyAdapter() {
          public void keyPressed(java.awt.event.KeyEvent evt) {
            if (evt.getKeyCode() == KeyEvent.VK_ENTER) {
              userInput = inputField.getText();
              System.out.println("User input: " + userInput);
              inputField.setText(""); // Clear the input field

              update();
            }
          }
        }
      );
      inputField.setFont(new Font("Monospaced", Font.PLAIN, 12)); // Use a monospaced font

      frame.add(new JScrollPane(terminalArea), BorderLayout.CENTER);
      frame.add(inputField, BorderLayout.SOUTH);
      frame.setVisible(true);

      System.out.println("Welcome to Wumpus.");
      update();
    });
  }

  public static void update() {
    String input = userInput;
    int direction = -1;
    switch (input) {
      case "0":
    case "top":
        direction = Wumpus.Direction.Top;
        break;
    case "1":
    case "upper right":
        direction = Wumpus.Direction.UpperRight;
        break;
    case "2":
    case "lower right":
        direction = Wumpus.Direction.LowerRight;
        break;
    case "3":
    case "bottom":
        direction = Wumpus.Direction.Bottom;
        break;
    case "4":
    case "lower left":
        direction = Wumpus.Direction.LowerLeft;
        break;
    case "5":
    case "upper left":
        direction = Wumpus.Direction.UpperLeft;
        break;
      case "":
        break;
      default:
        System.out.println(
          "Invalid input. Please enter a number between 0 and 5 or a direction (top, upper right, lower right, bottom, lower left, upper left)."
        );
        break;
    }

    if(direction>0 && direction<6){
      currentLocation = Wumpus.WumpusMap(currentLocation, direction);
    }

    System.out.println("You are now in room " + currentLocation);
  }
}

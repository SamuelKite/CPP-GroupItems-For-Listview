import React from "react";
import './Home.css';

export class Home extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            user: "dude"
        }
    }

    componentDidMount() {

    }

    componentDidUpdate(prevProps, prevState, snapshot) {
        if (prevState.user === "dude") {
            this.setState({ user: "whoa" });
        } else {
            this.setState({ user: "dude" });
        }
    }

    render() {
        const { user } = this.state;
        return (
            <div>
                {user}
            </div>
        )
    }
}

export default Home
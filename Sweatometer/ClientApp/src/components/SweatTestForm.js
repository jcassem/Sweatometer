import React, { Component } from 'react';

export class SweatTestForm extends Component {

    static firstWordName = "Parent Word";

    static secondWordName = "Inject Word";

    static answerWordName = "Proposed Pun";

    constructor(props) {
        super(props);

        this.handleChange = this.handleChange.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
    }

    handleChange(event) {
        this.props.onChange(event.target.name, event.target.value);
    }

    handleSubmit(event) {

        let errors = "";

        if (this.props.firstWord.length <= 2) {
            errors += SweatTestForm.firstWordName + " must be more than two characters long.\n";
        }
        if (this.props.secondWord.length <= 2) {
            errors += SweatTestForm.secondWordName + " must be more than two characters long.";
        }
        if (this.props.answerWord.length <= 2) {
            errors += SweatTestForm.answerWordName + " must be more than two characters long.";
        }

        if (errors.length > 0) {
            alert("Form contains errors:\n" + errors);
        }
        else {
            this.props.onSubmit();
        }

        event.preventDefault();
    }

    render() {
        return (
            <div>
                <div class="alert alert-light">
                    <p>How sweaty is your pun?</p>
                    <p>Where '<strong>{SweatTestForm.firstWordName}</strong>' + <strong>{SweatTestForm.secondWordName}</strong>' = <strong>{SweatTestForm.answerWordName}</strong></p>
                </div>
                <div>

                </div>
                <form onSubmit={this.handleSubmit} class="needs-validation" novalidate>
                    <div class="form-row">
                        <div class="col-md-4">
                            <label for="firstWord">{SweatTestForm.firstWordName}:</label>
                            <input type="text" class="form-control" id="firstWord" name="firstWord" value={this.props.firstWord} onChange={this.handleChange} />
                        </div>
                        <div class="col-md-4">
                            <label for="secondWord">{SweatTestForm.secondWordName}:</label>
                            <input type="text" class="form-control" id="secondWord" name="secondWord" value={this.props.secondWord} onChange={this.handleChange} />
                        </div>
                        <div class="col-md-4">
                            <label for="answerWord">{SweatTestForm.answerWordName}:</label>
                            <input type="text" class="form-control" id="answerWord" name="answerWord" value={this.props.answerWord} onChange={this.handleChange} />
                        </div>
                    </div>

                    <input type="submit" value="Search" class="btn btn-primary btn-lg btn-block submit" />
                </form>

            </div>
        );
  }
}

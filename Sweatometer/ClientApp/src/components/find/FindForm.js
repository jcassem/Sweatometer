import React, { Component } from 'react';

export class FindForm extends Component {

    static wordName = "Word";

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

        if (this.props.searchWord.length <= 2) {
            errors += FindForm.wordName + " must be more than two characters long.\n";
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
            <form onSubmit={this.handleSubmit}>
                <div class="form-row">
                    <div class="col-md-6 mb-6">
                        <label for="searchWord">{FindForm.wordName}:</label>
                        <input class="form-control" type="text" id="searchWord" name="searchWord" value={this.props.searchWord} onChange={this.handleChange} />
                    </div>

                    <div class="col-md-6 mb-6">
                        <label>Search Type:</label>
                        <select value={this.props.searchType} name="searchType"
                            onChange={this.handleChange} class="custom-select">
                            <option value="meansLike">Means Like</option>
                            <option value="soundsLike">Sounds Like</option>
                            <option value="spellsLike">Spells Like</option>
                            <option value="relatedTo">Related To</option>
                        </select>
                    </div>
                </div>

                <input type="submit" value="Search" class="btn btn-primary btn-lg btn-block submit" />
            </form>
        );
  }
}

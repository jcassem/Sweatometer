import React, { Component } from 'react';

export class FindForm extends Component {

    constructor(props) {
        super(props);

        this.handleChange = this.handleChange.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
    }

    handleChange(event) {
        this.props.onChange(event.target.name, event.target.value);
    }

    handleSubmit(event) {
        this.props.onSubmit();

        event.preventDefault();
    }

    render() {
        return (
            <form onSubmit={this.handleSubmit}>
                <div class="form-row">
                    <div class="col-md-6 mb-6">
                        <label>Word:</label>
                        <input class="form-control" type="text" name="wordToSoundLike" value={this.props.wordToSoundLike} onChange={this.handleChange} />
                    </div>

                    <div class="col-md-6 mb-6">
                        <label>Search Type:</label>
                        <select value={this.props.searchType} name="searchType"
                            onChange={this.handleChange} class="custom-select">
                            <option selected value="soundsLike">Sounds Like</option>
                            <option value="spellsLike">Spells Like</option>
                            <option selected value="meansLike">Means Like</option>
                        </select>
                    </div>
                </div>

                <input type="submit" value="Search" class="btn btn-primary btn-lg btn-block submit" />
            </form>
        );
  }
}
